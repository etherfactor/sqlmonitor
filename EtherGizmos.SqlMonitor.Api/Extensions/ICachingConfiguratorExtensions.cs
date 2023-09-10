using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using Medallion.Threading;
using Medallion.Threading.Redis;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Configuration;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

public static class ICachingConfiguratorExtensions
{
    public static ICachingConfigurator UsingInMemory(
        this ICachingConfigurator @this)
    {
        @this.UsingCache<InMemoryRecordCache>();

        return @this;
    }

    public static ICachingConfigurator UsingRedis(
        this ICachingConfigurator @this,
        IConfigurationSection section)
    {
        //Prepare Redis options
        var options = new ConfigurationOptions();
        section.Bind(options);

        var endpoints = section.GetSection("EndPoints").Get<RedisHost[]>()
            ?? Array.Empty<RedisHost>();
        foreach (var endpoint in endpoints)
        {
            options.EndPoints.Add(endpoint.Host, endpoint.Port);
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
}
