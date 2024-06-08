using EtherGizmos.SqlMonitor.Agent.Core.Services.Queries.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Messaging;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using MassTransit;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Messaging;

public class QueryExecuteConsumer : IConsumer<QueryExecuteMessage>
{
    private readonly IQueryRunnerFactory _queryRunnerFactory;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public QueryExecuteConsumer(
        IQueryRunnerFactory queryRunnerFactory,
        ISendEndpointProvider sendEndpointProvider)
    {
        _queryRunnerFactory = queryRunnerFactory;
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task Consume(
        ConsumeContext<QueryExecuteMessage> context)
    {
        var message = context.Message;

        var runner = await _queryRunnerFactory.GetRunnerAsync(
            message.MonitoredQueryTargetId,
            message.ConnectionRequestToken,
            message.SqlType);

        var result = await runner.ExecuteAsync(message, context.CancellationToken);

        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{MessagingConstants.Queues.CoordinatorQueryResult}"));
        await endpoint.Send(result);
    }
}
