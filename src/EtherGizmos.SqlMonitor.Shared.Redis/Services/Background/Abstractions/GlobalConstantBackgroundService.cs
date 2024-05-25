using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Services.Background.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Services.Background.Abstractions;

public abstract class GlobalConstantBackgroundService : PeriodicBackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDistributedLockProvider _distributedLockProvider;
    private readonly CrontabSchedule _schedule;

    private DateTime _lastRun;

    protected GlobalConstantBackgroundService(
        ILogger logger,
        IServiceProvider serviceProvider,
        IDistributedLockProvider distributedLockProvider,
        string lockCronExpression,
        string cronExpression)
        : base(logger, lockCronExpression)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _distributedLockProvider = distributedLockProvider;
        _schedule = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions()
        {
            IncludingSeconds = true
        });
    }

    /// <inheritdoc/>
    protected sealed override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        var key = new JobCacheKey(GetType());
        using var @lock = await _distributedLockProvider.AcquireLockAsync(key, TimeSpan.Zero, stoppingToken);

        //Return out if no lock was acquired, as another instance is handling this job
        if (@lock is null)
        {
            _logger.Log(LogLevel.Debug, "{ServiceName} - Failed to acquire lock, very likely due to another instance obtaining the lock first. This run will be skipped.", GetType().Name);
            return;
        }

        _logger.Log(LogLevel.Debug, "{ServiceName} - Successfully acquired lock. Job will run on this instance.", GetType().Name);

        //Continue running the job on this instance until a stop is requested or the lock is invalidated
        while (!stoppingToken.IsCancellationRequested && @lock.IsValid)
        {
            var now = DateTime.UtcNow;
            var nextTime = _schedule.GetNextOccurrence(DateTime.UtcNow);
            if (nextTime == _lastRun)
            {
                await Task.Delay(1);
                continue;
            }

            var nextWait = nextTime - now;

            await Task.Delay(nextWait, stoppingToken);

            _lastRun = nextTime;
            var scope = _serviceProvider.CreateScope().ServiceProvider;
            await DoConstantGlobalWorkAsync(scope, stoppingToken);
        }
    }

    /// <summary>
    /// Performs continuous background work on a single instance.
    /// </summary>
    /// <param name="scope">The scope for this run.</param>
    /// <param name="stoppingToken">The cancellation instruction.</param>
    /// <returns>An awaitable task.</returns>
    protected internal abstract Task DoConstantGlobalWorkAsync(IServiceProvider scope, CancellationToken stoppingToken);
}
