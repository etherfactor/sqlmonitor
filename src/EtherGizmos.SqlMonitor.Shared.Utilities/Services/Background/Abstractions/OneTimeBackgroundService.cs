namespace EtherGizmos.SqlMonitor.Shared.Utilities.Services.Background.Abstractions;

public abstract class OneTimeBackgroundService : BackgroundService
{
    private readonly ILogger _logger;

    public OneTimeBackgroundService(
        ILogger logger)
    {
        _logger = logger;
    }

    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
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
            throw;
        }
    }

    protected internal abstract Task DoWorkAsync(CancellationToken stoppingToken);
}
