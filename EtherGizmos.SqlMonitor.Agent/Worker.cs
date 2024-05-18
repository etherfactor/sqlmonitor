using EtherGizmos.SqlMonitor.Services.Background.Abstractions;

namespace EtherGizmos.SqlMonitor.Agent;

public class Worker : PeriodicBackgroundService
{
    private const string CronExpression = "*";

    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger) : base(logger, CronExpression)
    {
        _logger = logger;
    }

    protected override Task DoWorkAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
