using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Configuration;
using Medallion.Threading;
using Medallion.Threading.Redis;
using StackExchange.Redis;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

public static class ICachingConfiguratorExtensions
{
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

    /// <summary>
    /// Adds a generic cache.
    /// </summary>
    /// <typeparam name="TDistributedRecordCache">The type of cache.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>Itself.</returns>
    public static ICachingConfigurator UsingCache<TDistributedRecordCache>(
        this ICachingConfigurator @this)
        where TDistributedRecordCache : class, IDistributedRecordCache
    {
        @this.Services.AddSingleton<IDistributedRecordCache, TDistributedRecordCache>();

        return @this;
    }

    /// <summary>
    /// Adds an in-memory cache.
    /// </summary>
    /// <param name="this">Itself.</param>
    /// <returns>Itself.</returns>
    public static ICachingConfigurator UsingInMemory(
        this ICachingConfigurator @this)
    {
        @this.UsingCache<InMemoryRecordCache>();

        return @this;
    }

    /// <summary>
    /// Adds a Redis cache.
    /// </summary>
    /// <param name="this">Itself.</param>
    /// <param name="redisOptions">The options to configure Redis.</param>
    /// <returns>Itself.</returns>
    public static ICachingConfigurator UsingRedis(
        this ICachingConfigurator @this,
        RedisOptions redisOptions)
    {
        //Prepare Redis options
        var options = new ConfigurationOptions()
        {
            User = redisOptions.Username,
            Password = redisOptions.Password,
        };

        foreach (var endpoint in redisOptions.Hosts)
        {
            options.EndPoints.Add(endpoint.Address, endpoint.Port);
        }

        //Initialize the multiplexer and add it to the collection
        RedisConnectionMultiplexer.Initialize(options);
        var multiplexer = RedisConnectionMultiplexer.Instance;
        @this.Services.AddSingleton(e => multiplexer);

        //Add the distributed record cache
        @this.UsingCache<RedisDistributedRecordCache>();

        //Add the distributed locking provider
        @this.Services.AddSingleton<IDistributedLockProvider>(services =>
        {
            var multiplexer = services.GetRequiredService<IConnectionMultiplexer>();
            return new RedisDistributedSynchronizationProvider(multiplexer.GetDatabase());
        });

        return @this;
    }

    /// <summary>
    /// Forwards the cache services to the parent container.
    /// </summary>
    /// <param name="this">Itself.</param>
    /// <returns>Itself.</returns>
    public static IServiceCollectionChildExtensions.IChildContainerBuilder ForwardCaching(this IServiceCollectionChildExtensions.IChildContainerBuilder @this)
    {
        @this.ForwardSingleton<IDistributedRecordCache>();
        @this.ForwardSingleton<IRedisHelperFactory>();

        return @this;
    }
}
