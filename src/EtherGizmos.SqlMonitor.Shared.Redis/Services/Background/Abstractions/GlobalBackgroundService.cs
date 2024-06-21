using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Services.Background.Abstractions;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Services.Background.Abstractions;

/// <summary>
/// Performs work on a periodic timer. Uses distributed locks to run the job on the first instance to obtain its lock.
/// </summary>
public abstract class GlobalBackgroundService : PeriodicBackgroundService
{
    private readonly ILogger _logger;
    private readonly ILockingCoordinator _distributedLockProvider;

    public GlobalBackgroundService(
        ILogger logger,
        ILockingCoordinator distributedLockProvider,
        string cronExpression)
        : base(logger, cronExpression)
    {
        _logger = logger;
        _distributedLockProvider = distributedLockProvider;
    }

    /// <inheritdoc/>
    protected sealed override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        using var @lock = await _distributedLockProvider.AcquireLockAsync(
            new JobCacheKey(GetType()),
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
