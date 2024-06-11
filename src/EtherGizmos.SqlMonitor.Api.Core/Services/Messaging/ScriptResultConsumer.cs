using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.SqlMonitor.Api.Core.Services.Messaging;

public class ScriptResultConsumer : IConsumer<ScriptResultMessage>
{
    private readonly ILogger _logger;

    public ScriptResultConsumer(
        ILogger<ScriptResultConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<ScriptResultMessage> context)
    {
        var message = context.Message;

        _logger.LogInformation("Processing message {@ScriptResultMessage}", message);

        //Just want to confirm the message processes for now
        return Task.CompletedTask;
    }
}
