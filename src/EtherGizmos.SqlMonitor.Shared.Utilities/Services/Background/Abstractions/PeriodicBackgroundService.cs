using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace EtherGizmos.SqlMonitor.Shared.Utilities.Services.Background.Abstractions;

/// <summary>
/// Performs work on a periodic timer. Runs the job on every instance.
/// </summary>
public abstract class PeriodicBackgroundService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly CrontabSchedule _schedule;

    protected CrontabSchedule Schedule => _schedule;

    public PeriodicBackgroundService(
        ILogger logger,
        string cronExpression)
    {
        _logger = logger;
        _schedule = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions()
        {
            IncludingSeconds = true
        });
    }

    /// <inheritdoc/>
    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var firstDelay = GetNextDelay();
        await Task.Delay(firstDelay, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.Log(LogLevel.Debug, "{ServiceName} - Starting task run", GetType().Name);

                await DoWorkAsync(stoppingToken);

                _logger.Log(LogLevel.Debug, "{ServiceName} - Finishing task run", GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "{ServiceName} - Encountered an unexpected error while performing work", GetType().Name);
            }

            var nextDelay = GetNextDelay();
            await Task.Delay(nextDelay, stoppingToken);
        }
    }

    /// <summary>
    /// Performs background work.
    /// </summary>
    /// <param name="stoppingToken">The cancellation instruction.</param>
    /// <returns>An awaitable task.</returns>
    protected internal abstract Task DoWorkAsync(CancellationToken stoppingToken);

    /// <summary>
    /// Gets the next delay to run the job.
    /// </summary>
    /// <returns>The next delay.</returns>
    private int GetNextDelay()
    {
        //Wait to run until the next scheduled occurrence. Remove 100ms to add a bit of lenience, if the previous run
        //extended slightly past its scheduled duration.
        var occurrence = Schedule.GetNextOccurrence(DateTime.UtcNow.AddSeconds(-5)); //TODO: read from config and update comment
        var delay = (int)Math.Max(0, (occurrence - DateTime.UtcNow).TotalMilliseconds);

        return delay;
    }
}
