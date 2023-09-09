using EtherGizmos.SqlMonitor.Api.Consumers;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Helpers;
using EtherGizmos.SqlMonitor.Api.Services.Background.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;
using MassTransit;

namespace EtherGizmos.SqlMonitor.Api.Services.Background;

/// <summary>
/// Runs queries against instances on a periodic timer.
/// </summary>
public class EnqueueMonitorQueries : GlobalConstantBackgroundService
{
    private const string CronExpression = "0/15 * * * * *";
    private const string ConstantCronExpression = "0/1 * * * * *";

    private readonly ILogger _logger;

    private readonly RedisDistributedRecordCache _test;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="serviceProvider">Provides access to services.</param>
    public EnqueueMonitorQueries(
        ILogger<EnqueueMonitorQueries> logger,
        IServiceProvider serviceProvider,
        ILockedDistributedCache lockProvider,
        RedisDistributedRecordCache test)
        : base(logger, serviceProvider, lockProvider, CronExpression, ConstantCronExpression)
    {
        _logger = logger;
        _test = test;
    }

    protected override async Task DoConstantGlobalWorkAsync(IServiceProvider scope, CancellationToken stoppingToken)
    {
        var queryService = scope.GetRequiredService<IQueryService>();
        var allQueries = await queryService.GetOrLoadCacheAsync(stoppingToken);

        var instanceService = scope.GetRequiredService<IInstanceService>();
        var allInstances = await instanceService.GetOrLoadCacheAsync(stoppingToken);

        await ScheduleQueriesAsync(scope, allInstances, allQueries, stoppingToken);
    }

    private async Task ScheduleQueriesAsync(
        IServiceProvider scope,
        IEnumerable<Instance> allInstances,
        IEnumerable<Query> allQueries,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var queries = allQueries
            //Convert the runtime to an age based on the current time.
            .Where(e => now - (e.LastRunAtUtc ?? DateTimeOffset.MinValue)
                //Compare to the run frequency, but shave off a small portion to avoid waiting a whole extra run cycle if
                //the cycle is off by a few milliseconds.
                >= e.RunFrequency - DateAndTimeHelper.Min(0.05 * e.RunFrequency, TimeSpan.FromMilliseconds(100)));

        //Prepare to run only the queries due to be run, on all instances.
        //TODO: account for query whitelists/blacklists.
        var instanceQueries = allInstances.CrossJoin(queries);

        var sendEndpointProvider = scope.GetRequiredService<ISendEndpointProvider>();
        var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RunQueryConsumer.Queue}"));

        await Parallel.ForEachAsync(
            instanceQueries,
            new ParallelOptions()
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = 8
            },
            async (instanceQuery, cancellationToken) =>
            {
                var instance = instanceQuery.Item1;
                var query = instanceQuery.Item2;

                await endpoint.Send(new RunQuery()
                {
                    Instance = instance,
                    Query = query,
                }, cancellationToken);

                query.LastRunAtUtc = DateTimeOffset.UtcNow;

                var updateScope = scope.CreateScope().ServiceProvider;
                var saveService = updateScope.GetRequiredService<ISaveService>();
                saveService.Attach(query);

                await saveService.SaveChangesAsync();
            });

        var distributedCache = scope.GetRequiredService<ILockedDistributedCache>();
        using var @lock = await distributedCache.AcquireLockAsync(CacheKeys.AllQueries, TimeSpan.FromSeconds(1), cancellationToken);
        if (@lock is not null)
        {
            await distributedCache.SetWithLockAsync(CacheKeys.AllQueries, @lock, allQueries.ToList(), cancellationToken);
        }
        else
        {
            _logger.Log(LogLevel.Warning, "Expected to receive a lock for {CacheKey}, but the request timed out.", nameof(CacheKeys.AllQueries));
        }
    }
}
