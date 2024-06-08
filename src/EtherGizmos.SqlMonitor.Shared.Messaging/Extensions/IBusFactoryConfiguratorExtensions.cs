using MassTransit;
using RabbitMQ.Client;

namespace EtherGizmos.SqlMonitor.Shared.Messaging.Extensions;

public static class IBusFactoryConfiguratorExtensions
{
    public static void ReceiveQueue<TConsumer>(this IBusFactoryConfigurator @this, IBusRegistrationContext context, string queueName, Action<IReceiveEndpointConfigurator>? configureEndpoint = null)
        where TConsumer : class, IConsumer
    {
        void useConfigureEndpoint(IReceiveEndpointConfigurator opt)
        {
            configureEndpoint?.Invoke(opt);
            opt.Consumer<TConsumer>(context);
        }

        if (@this is IBusFactoryConfigurator<IRabbitMqReceiveEndpointConfigurator> rabbitMq)
        {
            rabbitMq.ReceiveEndpoint(queueName, useConfigureEndpoint);
        }
        else
        {
            @this.ReceiveEndpoint(queueName, useConfigureEndpoint);
        }
    }

    public static void ReceiveTopic<TConsumer>(this IBusFactoryConfigurator @this, IBusRegistrationContext context, string topicName, string subscriptionName, Action<IReceiveEndpointConfigurator>? configureEndpoint = null)
        where TConsumer : class, IConsumer
    {
        void useConfigureEndpoint(IReceiveEndpointConfigurator opt)
        {
            configureEndpoint?.Invoke(opt);
            opt.Consumer<TConsumer>(context);
        }

        if (@this is IBusFactoryConfigurator<IRabbitMqReceiveEndpointConfigurator> rabbitMq)
        {
            rabbitMq.ReceiveEndpoint(subscriptionName, opt =>
            {
                opt.Bind(topicName, x =>
                {
                    x.ExchangeType = ExchangeType.Fanout;
                });

                useConfigureEndpoint(opt);
            });
        }
        else
        {
            @this.ReceiveEndpoint(topicName, useConfigureEndpoint);
        }
    }
}
