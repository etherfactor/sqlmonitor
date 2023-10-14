using MassTransit;
using MassTransit.RabbitMqTransport;
using RabbitMQ.Client;

namespace EtherGizmos.SqlMonitor.Api.Services.Messaging;

public class RabbitMqTopicManager
{
    private IRabbitMqHost _host;

    public RabbitMqTopicManager(IRabbitMqHost host)
    {
        _host = host;
    }

    public async Task Subscribe<TConsumer>(string queueName, string routingKey)
        where TConsumer : class, IConsumer, new()
    {
        var endpoint = _host.ConnectReceiveEndpoint(queueName, opt =>
        {
            opt.ExchangeType = ExchangeType.Topic;
            opt.Consumer<TConsumer>();
            opt.Bind($"exchange:{queueName}", conf => conf.RoutingKey = routingKey);
        });

        await endpoint.Ready;
    }

    public async Task Unsubscribe<TConsumer>(string queueName, string routingKey)
        where TConsumer : class, IConsumer, new()
    {
        //TODO
    }
}
