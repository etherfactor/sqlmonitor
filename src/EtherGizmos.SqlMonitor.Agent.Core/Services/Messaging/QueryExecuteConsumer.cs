using EtherGizmos.SqlMonitor.Agent.Core.Services.Queries.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using MassTransit;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Messaging;

public class QueryExecuteConsumer : IConsumer<QueryExecuteMessage>
{
    private readonly IQueryRunnerFactory _queryRunnerFactory;

    public QueryExecuteConsumer(
        IQueryRunnerFactory queryRunnerFactory)
    {
        _queryRunnerFactory = queryRunnerFactory;
    }

    public async Task Consume(
        ConsumeContext<QueryExecuteMessage> context)
    {
        var message = context.Message;

        var runner = await _queryRunnerFactory.GetRunnerAsync(
            message.MonitoredQueryTargetId,
            message.ConnectionRequestToken,
            message.SqlType);

        await runner.ExecuteAsync(message, context.CancellationToken);
    }
}
