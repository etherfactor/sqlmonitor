namespace EtherGizmos.SqlMonitor.Api.Services.Abstractions;

public abstract class PeriodicBackgroundService : BackgroundService
{
    private ILogger Logger { get; }

    protected abstract TimeSpan Period { get; }

    public PeriodicBackgroundService(ILogger logger)
    {
        Logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(Period);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await DoWorkAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex, "{ServiceName} - Encountered an unexpected error while performing work", GetType().Name);
            }
        }
    }

    protected abstract Task DoWorkAsync(CancellationToken stoppingToken);
}
