using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using MassTransit;

namespace EtherGizmos.SqlMonitor.Api.Core.Services.Messaging;

public class ScriptResultConsumer : IConsumer<ScriptResultMessage>
{
    public Task Consume(ConsumeContext<ScriptResultMessage> context)
    {
        throw new NotImplementedException();
    }
}
