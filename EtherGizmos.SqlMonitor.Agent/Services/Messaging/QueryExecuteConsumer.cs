using EtherGizmos.SqlMonitor.Agent.Services.Queries.Abstractions;
using EtherGizmos.SqlMonitor.Models.Messaging;
using MassTransit;

namespace EtherGizmos.SqlMonitor.Agent.Services.Messaging;

internal class QueryExecuteConsumer : IConsumer<QueryExecuteMessage>
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
            message.QueryVariant.SqlType);

        await runner.ExecuteAsync(message.QueryVariant, context.CancellationToken);
    }
}
