using EtherGizmos.SqlMonitor.Api.Core.Helpers;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Messaging;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Redis.Services.Background.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Extensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.SqlMonitor.Api.Core.Services.Background;

/// <summary>
/// Runs queries against instances on a periodic timer.
/// </summary>
public class EnqueueQueryMessagesService : GlobalConstantBackgroundService
{
    private const string CronExpression = "0/15 * * * * *";
    private const string ConstantCronExpression = "0/1 * * * * *";

    private readonly ILogger _logger;
    private readonly ILockingCoordinator _distributedLockProvider;
    private readonly IRecordCache _distributedRecordCache;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="serviceProvider">Provides access to services.</param>
    public EnqueueQueryMessagesService(
        ILogger<EnqueueQueryMessagesService> logger,
        IServiceProvider serviceProvider,
        ILockingCoordinator distributedLockProvider,
        IRecordCache distributedRecordCache)
        : base(logger, serviceProvider, distributedLockProvider, CronExpression, ConstantCronExpression)
    {
        _logger = logger;
        _distributedLockProvider = distributedLockProvider;
        _distributedRecordCache = distributedRecordCache;
    }

    /// <inheritdoc/>
    protected override async Task DoConstantGlobalWorkAsync(IServiceProvider provider, CancellationToken stoppingToken)
    {
        var now = DateTimeOffset.Now;
        var loadTargetTasks = new Dictionary<SqlType, Task<List<MonitoredQueryTarget>>>();

        var querySet = _distributedRecordCache.EntitySet<Query>();
        var monitoredQueryTargetSet = _distributedRecordCache.EntitySet<MonitoredQueryTarget>();

        var queriesToRun = await querySet.Where(e => e.NextRunAtUtc)
            .IsLessThanOrEqualTo(DateTimeOffset.UtcNow.Round(TimeSpan.FromSeconds(1)))
            .ToListAsync(stoppingToken);

        var sendEndpointProvider = provider.GetRequiredService<ISendEndpointProvider>();
        var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{MessagingConstants.Queues.AgentQueryExecute}"));

        var queryVariantsToRun = queriesToRun.SelectMany(e => e.Variants)
            .ToList();

        //Return early if there's nothing to run
        if (queryVariantsToRun.Count == 0)
            return;

        _logger.LogInformation("Identified {QueryCount} queries scheduled to run", queriesToRun.Count);

        foreach (var queryVariant in queryVariantsToRun)
        {
            if (!loadTargetTasks.ContainsKey(queryVariant.SqlType))
            {
                loadTargetTasks.Add(queryVariant.SqlType, monitoredQueryTargetSet
                    .Where(e => e.SqlType)
                    .IsEqualTo(queryVariant.SqlType)
                    .ToListAsync(stoppingToken));
            }

            var instancesToTarget = await loadTargetTasks[queryVariant.SqlType];

            foreach (var instance in instancesToTarget)
            {
                var message = new QueryExecuteMessage(
                    queryVariant.Query.Id,
                    queryVariant.Query.Name,
                    instance.Id,
                    ConnectionTokenHelper.CreateFor(instance, now.Add(queryVariant.Query.RunFrequency)),
                    queryVariant.SqlType,
                    queryVariant.QueryText,
                    queryVariant.Query.BucketColumn,
                    queryVariant.Query.TimestampUtcColumn);

                foreach (var queryMetric in queryVariant.Query.Metrics)
                {
                    message.AddMetric(queryMetric.MetricId, queryMetric.ValueColumn);
                }

                await sendEndpoint.Send(message, stoppingToken);
            }
        }

        var parallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 8,
        };

        await Parallel.ForEachAsync(queriesToRun, parallelOptions, async (query, cancellationToken) =>
        {
            using var subScope = provider.CreateScope();
            var subProvider = subScope.ServiceProvider;

            var queryService = subProvider.GetRequiredService<IQueryService>();
            queryService.Add(query);

            var saveService = provider.GetRequiredService<ISaveService>();
            await saveService.SaveChangesAsync();

            query.LastRunAtUtc = DateTimeOffset.UtcNow.Round(TimeSpan.FromSeconds(1));
            await querySet.AddAsync(query, stoppingToken);
        });
    }
}
