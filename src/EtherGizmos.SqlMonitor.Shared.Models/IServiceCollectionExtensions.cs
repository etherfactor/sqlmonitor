using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EtherGizmos.SqlMonitor.Shared.Models;

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
        @this.TryAddSingleton((provider) =>
        {
            MapperConfiguration configuration = new MapperConfiguration(opt =>
            {
                //Add enums
                opt.AddExecType();
                opt.AddAggregateType();
                opt.AddSqlType();

                //Add entities
                opt.AddMetric();
                opt.AddMonitoredEnvironment();
                opt.AddMonitoredQueryTarget();
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
