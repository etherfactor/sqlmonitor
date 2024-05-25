﻿namespace EtherGizmos.SqlMonitor.Api.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds AutoMapper to the service collection.
    /// </summary>
    /// <param name="this">Itself.</param>
    /// <returns>Itself.</returns>
    public static IServiceCollection AddMapper(this IServiceCollection @this)
    {
        @this.AddSingleton<IMapper>((provider) =>
        {
            MapperConfiguration configuration = new MapperConfiguration(opt =>
            {
                //Add enums
                opt.AddAggregateType();

                //Add entities
                opt.AddMetric();
                opt.AddMonitoredEnvironment();
                opt.AddMonitoredResource();
                opt.AddMonitoredScriptTarget();
                opt.AddMonitoredSystem();
                opt.AddQuery();
                opt.AddQueryMetric();
                opt.AddQueryVariant();
                opt.AddScript();
                opt.AddScriptInterpreter();
                opt.AddScriptMetric();
                opt.AddScriptVariant();
            });

            return configuration.CreateMapper();
        });

        return @this;
    }
}