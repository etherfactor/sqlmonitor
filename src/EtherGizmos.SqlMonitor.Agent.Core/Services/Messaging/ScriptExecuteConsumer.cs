using EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Messaging;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Messaging;

public class ScriptExecuteConsumer : IConsumer<ScriptExecuteMessage>
{
    private readonly ILogger _logger;
    private readonly IScriptRunnerFactory _scriptRunnerFactory;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public ScriptExecuteConsumer(
        ILogger<ScriptExecuteConsumer> logger,
        IScriptRunnerFactory scriptRunnerFactory,
        ISendEndpointProvider sendEndpointProvider)
    {
        _logger = logger;
        _scriptRunnerFactory = scriptRunnerFactory;
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task Consume(
        ConsumeContext<ScriptExecuteMessage> context)
    {
        var message = context.Message;

        _logger.LogInformation("Processing message {@ScriptExecuteMessage}", message);

        var runner = await _scriptRunnerFactory.GetRunnerAsync(
            message.MonitoredScriptTargetId,
            message.ConnectionRequestToken,
            message.ExecType);

        var result = await runner.ExecuteAsync(message, context.CancellationToken);

        _logger.LogInformation("Returning message {@ScriptResultMessage}", result);

        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{MessagingConstants.Queues.CoordinatorScriptResult}"));
        await endpoint.Send(result);
    }
}
