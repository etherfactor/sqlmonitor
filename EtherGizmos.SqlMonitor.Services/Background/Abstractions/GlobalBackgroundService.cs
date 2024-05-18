using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

namespace EtherGizmos.SqlMonitor.Services.Background.Abstractions;

/// <summary>
/// Performs work on a periodic timer. Uses distributed locks to run the job on the first instance to obtain its lock.
/// </summary>
public abstract class GlobalBackgroundService : PeriodicBackgroundService
{
    private readonly ILogger _logger;
    private readonly IDistributedRecordCache _distributedRecordCache;

    public GlobalBackgroundService(
        ILogger logger,
        IDistributedRecordCache distributedRecordCache,
        string cronExpression)
        : base(logger, cronExpression)
    {
        _logger = logger;
        _distributedRecordCache = distributedRecordCache;
    }

    /// <inheritdoc/>
    protected internal sealed override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        using var @lock = await _distributedRecordCache.AcquireLockAsync(
            CacheKeys.EnqueueMonitorQueries,
            timeout: TimeSpan.Zero,
            cancellationToken: stoppingToken);

        if (@lock is not null)
        {
            _logger.Log(LogLevel.Debug, "{ServiceName} - Successfully acquired lock. Job will run on this instance.", GetType().Name);

            await DoGlobalWorkAsync(stoppingToken);
        }
        else
        {
            _logger.Log(LogLevel.Debug, "{ServiceName} - Failed to acquire lock, very likely due to another instance obtaining the lock first. This run will be skipped.", GetType().Name);
        }
    }

    /// <summary>
    /// Performs background work on a single instance.
    /// </summary>
    /// <param name="stoppingToken">The cancellation instruction.</param>
    /// <returns>An awaitable task.</returns>
    protected internal abstract Task DoGlobalWorkAsync(CancellationToken stoppingToken);
}
