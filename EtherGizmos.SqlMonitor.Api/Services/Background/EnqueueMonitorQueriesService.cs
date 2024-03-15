using EtherGizmos.SqlMonitor.Api.Services.Background.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

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

    }
}
