using EtherGizmos.Extensions.DependencyInjection;
using EtherGizmos.SqlMonitor.Shared.Configuration;
using EtherGizmos.SqlMonitor.Shared.Configuration.Messaging;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Shared.Messaging;

public static class IServiceCollectionExtensions
{
    public static IChildContainerBuilder AddConfiguredMassTransit(
        this IServiceCollection @this,
        Action<IBusRegistrationContext, IBusFactoryConfigurator> configureEndpoints,
        params Assembly[] consumerAssemblies)
    {
        var builder = @this.AddChildContainer(
            (childServices, parentServices) =>
            {
                var usageOptions = parentServices
                    .GetRequiredService<IOptions<UsageOptions>>()
                    .Value;

                childServices.AddMassTransit(opt =>
                {
                    opt.AddConsumers(consumerAssemblies);

                    if (usageOptions.MessageBroker == MessageBrokerType.InMemory)
                    {
                        opt.UsingInMemory((context, conf) =>
                        {
                            conf.Host();

                            configureEndpoints(context, conf);

                            //TODO: Configure in-memory retry and other options
                        });
                    }
                    else if (usageOptions.MessageBroker == MessageBrokerType.RabbitMQ)
                    {
                        var rabbitMQOptions = parentServices
                            .GetRequiredService<IOptions<RabbitMQOptions>>()
                            .Value;

                        opt.UsingRabbitMq((context, conf) =>
                        {
                            string useHost;
                            if (rabbitMQOptions.Hosts.Count == 1)
                            {
                                var host = rabbitMQOptions.Hosts.Single();

                                useHost = host.Address;
                                var usePort = host.Port != 0 ? host.Port : MessagingConstants.RabbitMQ.Port;

                                useHost = $"{useHost}:{usePort}";
                            }
                            else
                            {
                                useHost = "cluster";
                            }

                            conf.Host(useHost, opt =>
                            {
                                opt.Username(rabbitMQOptions.Username);
                                opt.Password(rabbitMQOptions.Password);

                                if (rabbitMQOptions.Hosts.Count > 1)
                                {
                                    opt.UseCluster(conf =>
                                    {
                                        foreach (var node in rabbitMQOptions.Hosts)
                                        {
                                            var useNode = node.Address;
                                            var usePort = node.Port != 0 ? node.Port : MessagingConstants.RabbitMQ.Port;

                                            conf.Node(useNode);
                                        }
                                    });
                                }
                            });

                            configureEndpoints(context, conf);

                            //TODO: Configure retry
                        });
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("Unknown message broker type: {0}", usageOptions.MessageBroker));
                    }
                });
            })
            .ImportLogging()
            .ForwardMassTransit();

        return builder;
    }

    /// <summary>
    /// Forwards the MassTransit services to the parent container.
    /// </summary>
    /// <param name="this">Itself.</param>
    /// <returns>Itself.</returns>
    private static IChildContainerBuilder ForwardMassTransit(this IChildContainerBuilder @this)
    {
        @this.ForwardSingleton<IHostedService>();
        @this.ForwardScoped<ISendEndpointProvider>();

        return @this;
    }
}
