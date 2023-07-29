namespace EtherGizmos.SqlMonitor.Api.Services.Abstractions;

/// <summary>
/// Performs work on a periodic timer.
/// </summary>
public abstract class PeriodicBackgroundService : BackgroundService
{
    /// <summary>
    /// The logger to use.
    /// </summary>
    private ILogger Logger { get; }

    /// <summary>
    /// The frequency at which to run the task.
    /// </summary>
    protected abstract TimeSpan Period { get; }

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger"></param>
    public PeriodicBackgroundService(ILogger logger)
    {
        Logger = logger;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(Period);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                Logger.Log(LogLevel.Debug, "{ServiceName} - Starting task run", GetType().Name);

                await DoWorkAsync(stoppingToken);

                Logger.Log(LogLevel.Debug, "{ServiceName} - Finishing task run", GetType().Name);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex, "{ServiceName} - Encountered an unexpected error while performing work", GetType().Name);
            }
        }
    }

    /// <summary>
    /// Performs background work.
    /// </summary>
    /// <param name="stoppingToken">The cancellation instruction.</param>
    /// <returns>An awaitable task.</returns>
    protected abstract Task DoWorkAsync(CancellationToken stoppingToken);
}
