using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using NCrontab;

namespace EtherGizmos.SqlMonitor.Api.Services.Background.Abstractions;

public abstract class GlobalConstantBackgroundService : GlobalBackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly CrontabSchedule _constantSchedule;

    protected GlobalConstantBackgroundService(
        ILogger logger,
        IServiceProvider serviceProvider,
        ILockedDistributedCache lockProvider,
        string cronExpression,
        string constantCronExpression)
        : base(logger, lockProvider, cronExpression)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _constantSchedule = CrontabSchedule.Parse(constantCronExpression, new CrontabSchedule.ParseOptions()
        {
            IncludingSeconds = true
        });
    }

    protected sealed override async Task DoGlobalWorkAsync(CancellationToken stoppingToken)
    {
        var scope = _serviceProvider.CreateScope().ServiceProvider;

        var startTime = DateTime.UtcNow - TimeSpan.FromSeconds(5); //TODO: read from config
        var startOccurrence = Schedule.GetNextOccurrence(startTime);
        var nextOccurrence = Schedule.GetNextOccurrence(startOccurrence.AddMilliseconds(1));

        var subOccurrences = _constantSchedule.GetNextOccurrences(startOccurrence.AddMilliseconds(-1), nextOccurrence.AddMilliseconds(-1));
        var delays = subOccurrences.Select(e => (int)Math.Max(0, (e - DateTime.UtcNow).TotalMilliseconds))
            .Distinct();

        await Parallel.ForEachAsync(delays, async (delay, cancellationToken) =>
        {
            await Task.Delay(delay, cancellationToken);

            _logger.Log(LogLevel.Debug, "{ServiceName} - Running constant work", GetType().Name);
            await DoConstantGlobalWorkAsync(scope, cancellationToken);
        });
    }

    protected abstract Task DoConstantGlobalWorkAsync(IServiceProvider scope, CancellationToken stoppingToken);
}
