using MassTransit;

namespace EtherGizmos.SqlMonitor.Api.Consumers;

public class RunQueryConsumer : IConsumer<RunQuery>
{
    public const string Queue = "run-query";

    private ILogger Logger { get; }

    public RunQueryConsumer(ILogger<RunQueryConsumer> logger)
    {
        Logger = logger;
    }

    public Task Consume(ConsumeContext<RunQuery> context)
    {
        Logger.Log(LogLevel.Information, "Received data: {Data}", context.Message);
        return Task.CompletedTask;
    }
}

public class RunQuery
{
    public Guid InstanceId { get; set; }

    public Guid QueryId { get; set; }
}
