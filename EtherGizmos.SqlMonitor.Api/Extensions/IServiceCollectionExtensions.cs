using AutoMapper;
using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Models.Api.v1;

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
                opt.AddInstance();
                opt.AddInstanceQuery();
                opt.AddInstanceQueryDatabase();
                opt.AddPermission();
                opt.AddSecurable();
                opt.AddQuery();
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
