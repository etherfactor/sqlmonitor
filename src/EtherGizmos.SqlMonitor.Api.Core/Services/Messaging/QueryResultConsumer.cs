using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.SqlMonitor.Api.Core.Services.Messaging;

public class QueryResultConsumer : IConsumer<QueryResultMessage>
{
    private readonly ILogger _logger;

    public QueryResultConsumer(
        ILogger<QueryResultConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<QueryResultMessage> context)
    {
        var message = context.Message;

        _logger.LogInformation("Processing message {@QueryResultMessage}", message);

        //Just want to confirm the message processes for now
        return Task.CompletedTask;
    }
}
