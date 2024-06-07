using EtherGizmos.SqlMonitor.Api.Core.Helpers;
using EtherGizmos.SqlMonitor.Shared.Messaging;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Redis.Services.Background.Abstractions;
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
    private readonly IServiceProvider _serviceProvider;
    private readonly IDistributedLockProvider _distributedLockProvider;
    private readonly IDistributedRecordCache _distributedRecordCache;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="serviceProvider">Provides access to services.</param>
    public EnqueueQueryMessagesService(
        ILogger<EnqueueQueryMessagesService> logger,
        IServiceProvider serviceProvider,
        IDistributedLockProvider distributedLockProvider,
        IDistributedRecordCache distributedRecordCache)
        : base(logger, serviceProvider, distributedLockProvider, CronExpression, ConstantCronExpression)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _distributedLockProvider = distributedLockProvider;
        _distributedRecordCache = distributedRecordCache;
    }

    /// <inheritdoc/>
    protected override async Task DoConstantGlobalWorkAsync(IServiceProvider scope, CancellationToken stoppingToken)
    {
        var now = DateTimeOffset.Now;
        var loadTargetTasks = new Dictionary<SqlType, Task<List<MonitoredQueryTarget>>>();

        var querySet = _distributedRecordCache.EntitySet<Query>();
        var monitoredQueryTargetSet = _distributedRecordCache.EntitySet<MonitoredQueryTarget>();

        var queriesToRun = await querySet.Where(e => e.NextRunAtUtc)
            .IsLessThanOrEqualTo(DateTimeOffset.UtcNow)
            .ToListAsync(stoppingToken);

        var sendEndpointProvider = _serviceProvider.GetRequiredService<ISendEndpointProvider>();
        var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{MessagingConstants.Queues.CoordinatorAgentQueryExecute}"));

        foreach (var queryVariant in queriesToRun.SelectMany(e => e.Variants))
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

                await sendEndpoint.Send(message, stoppingToken);
            }
        }
    }
}
