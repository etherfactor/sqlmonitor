using EtherGizmos.SqlMonitor.Api.Consumers;
using EtherGizmos.SqlMonitor.Api.Jobs.Abstractions;
using MassTransit;

namespace EtherGizmos.SqlMonitor.Api.Jobs;

public class EnqueueMonitorQueries : IEnqueueMonitorQueries
{
    private ILogger Logger { get; }

    private ISendEndpointProvider EndpointProvider { get; }

    public EnqueueMonitorQueries(ILogger<EnqueueMonitorQueries> logger, ISendEndpointProvider endpointProvider)
    {
        Logger = logger;
        EndpointProvider = endpointProvider;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        Logger.Log(LogLevel.Information, "Running Hangfire job");

        var endpoint = await EndpointProvider.GetSendEndpoint(new Uri("queue:run-query"));
        await endpoint.Send(new RunQuery()
        {
            InstanceId = Guid.NewGuid(),
            QueryId = Guid.NewGuid()
        }, cancellationToken);
    }
}
