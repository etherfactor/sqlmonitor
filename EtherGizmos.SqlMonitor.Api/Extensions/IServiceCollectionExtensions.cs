using AutoMapper;
using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Models.Api.v1;
using EtherGizmos.SqlMonitor.Models.Api.v1.Enums;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

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
                opt.AddSeverityType();

                //Add entities
                opt.AddInstance();
                opt.AddMetric();
                opt.AddMetricSeverity();
                opt.AddQueryInstance();
                opt.AddQueryInstanceDatabase();
                opt.AddQueryMetric();
                opt.AddQueryMetricSeverity();
                opt.AddPermission();
                opt.AddQuery();
                opt.AddSecurable();
                opt.AddUser();
            });

            return configuration.CreateMapper();
        });

        return @this;
    }

    /// <summary>
    /// Adds distributed caching to the service collection. Requires additional calls to <paramref name="configure"/>
    /// to add the cache.
    /// </summary>
    /// <param name="this">Itself.</param>
    /// <param name="configure">The action to configure the cache.</param>
    /// <returns>Itself.</returns>
    public static IServiceCollection AddCaching(this IServiceCollection @this, Action<ICachingConfigurator> configure)
    {
        var configurator = new CachingConfigurator(@this);

        configure(configurator);

        return @this;
    }
}
