using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Background.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Messaging;
using EtherGizmos.SqlMonitor.Models.Database;
using MassTransit;

namespace EtherGizmos.SqlMonitor.Api.Services.Background;

/// <summary>
/// Runs queries against instances on a periodic timer.
/// </summary>
public class EnqueueMonitorQueriesService : GlobalConstantBackgroundService
{
    private const string CronExpression = "0/15 * * * * *";
    private const string ConstantCronExpression = "0/1 * * * * *";

    private readonly ILogger _logger;
    private readonly IDistributedRecordCache _distributedRecordCache;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="serviceProvider">Provides access to services.</param>
    public EnqueueMonitorQueriesService(
        ILogger<EnqueueMonitorQueriesService> logger,
        IServiceProvider serviceProvider,
        IDistributedRecordCache distributedRecordCache)
        : base(logger, serviceProvider, distributedRecordCache, CacheKeys.EnqueueMonitorQueries, CronExpression, ConstantCronExpression)
    {
        _logger = logger;
        _distributedRecordCache = distributedRecordCache;
    }

    /// <inheritdoc/>
    protected internal override async Task DoConstantGlobalWorkAsync(IServiceProvider scope, CancellationToken stoppingToken)
    {
        var queriesToRun = await _distributedRecordCache.EntitySet<Query>()
            .Where(e => e.IsActive).IsEqualTo(true)
            .Where(e => e.NextRunAtUtc).IsLessThanOrEqualTo(DateTimeOffset.UtcNow)
            .ToListAsync();

        var allInstances = await _distributedRecordCache.EntitySet<Instance>()
            .Where(e => e.IsActive).IsEqualTo(true)
            .ToListAsync();

        var instanceQueries = allInstances.CrossJoin(queriesToRun);

        var allMetrics = new List<List<QueryMetric>>();
        foreach (var queryToRun in queriesToRun)
        {
            allMetrics.Add(queryToRun.Metrics);
        }

        var sendEndpointProvider = scope.GetRequiredService<ISendEndpointProvider>();
        var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RunQueryConsumer.Queue}"));

        await Parallel.ForEachAsync(
            instanceQueries,
            new ParallelOptions()
            {
                CancellationToken = stoppingToken,
                MaxDegreeOfParallelism = 8
            },
            async (instanceQuery, cancellationToken) =>
            {
                var instance = instanceQuery.Item1;
                var query = instanceQuery.Item2;

                await endpoint.Send(new RunQuery()
                {
                    Instance = instance,
                    Query = query
                }, cancellationToken);

                var updateScope = scope.CreateScope().ServiceProvider;
                var saveService = updateScope.GetRequiredService<ISaveService>();
                saveService.Attach(query);

                query.LastRunAtUtc = DateTimeOffset.UtcNow.Floor(TimeSpan.FromSeconds(1));

                await saveService.SaveChangesAsync();
                await _distributedRecordCache.EntitySet<Query>().AddAsync(query, cancellationToken);
            });
    }
}
