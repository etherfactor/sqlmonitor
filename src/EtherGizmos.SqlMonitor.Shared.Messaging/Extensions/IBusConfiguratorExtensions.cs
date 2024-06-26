﻿using EtherGizmos.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace EtherGizmos.SqlMonitor.Shared.Messaging.Extensions;

public static class IBusConfiguratorExtensions
{
    /// <summary>
    /// Forwards the MassTransit services to the parent container.
    /// </summary>
    /// <param name="this">Itself.</param>
    /// <returns>Itself.</returns>
    public static IChildContainerBuilder ForwardMassTransit(this IChildContainerBuilder @this)
    {
        @this.ForwardSingleton<IHostedService>();
        @this.ForwardScoped<ISendEndpointProvider>();

        return @this;
    }
}
