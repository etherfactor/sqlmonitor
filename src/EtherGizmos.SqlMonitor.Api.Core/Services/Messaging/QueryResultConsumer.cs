using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using MassTransit;

namespace EtherGizmos.SqlMonitor.Api.Core.Services.Messaging;

public class QueryResultConsumer : IConsumer<QueryResultMessage>
{
    public Task Consume(ConsumeContext<QueryResultMessage> context)
    {
        throw new NotImplementedException();
    }
}
