using EtherGizmos.Extensions.DependencyInjection;
using EtherGizmos.SqlMonitor.Shared.Configuration;
using EtherGizmos.SqlMonitor.Shared.Configuration.Caching;
using EtherGizmos.SqlMonitor.Shared.Redis.Caching;
using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Redis.Locking;
using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Extensions;
using Medallion.Threading.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.System.Text.Json;
using IMedallionDistributedLockProvider = Medallion.Threading.IDistributedLockProvider;

namespace EtherGizmos.SqlMonitor.Shared.Redis;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedCaching(this IServiceCollection @this)
    {
        @this
            .AddChildContainer((childServices, parentServices) =>
            {
                var usageOptions = parentServices
                    .GetRequiredService<IOptions<UsageOptions>>()
                    .Value;

                if (usageOptions.Cache == CacheType.InMemory)
                {
                    childServices.AddSingleton<IDistributedRecordCache, InMemoryRecordCache>();
                }
                else if (usageOptions.Cache == CacheType.Redis)
                {
                    childServices.AddRedisServices();
                    childServices.AddSingleton<IDistributedRecordCache, RedisDistributedRecordCache>();
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unknown cache type: {0}", usageOptions.Cache));
                }

                childServices.AddSingleton<IRedisHelperFactory>(e => RedisHelperFactory.Instance);
            })
            .ImportLogging()
            .ImportSingleton<IOptions<UsageOptions>>()
            .ForwardSingleton<IDistributedRecordCache>()
            .ForwardSingleton<IRedisHelperFactory>();

        return @this;
    }

    public static IServiceCollection AddDistributedLocking(this IServiceCollection @this)
    {
        @this
            .AddChildContainer((childServices, parentServices) =>
            {
                var usageOptions = parentServices
                    .GetRequiredService<IOptions<UsageOptions>>()
                    .Value;

                if (usageOptions.Cache == CacheType.InMemory)
                {
                    childServices.AddSingleton<IDistributedLockProvider, InMemoryLockProvider>();
                }
                else if (usageOptions.Cache == CacheType.Redis)
                {
                    childServices.AddSingleton<IMedallionDistributedLockProvider, RedisDistributedSynchronizationProvider>();
                    childServices.AddSingleton<IDistributedLockProvider, DistributedLockProvider>();
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unknown cache type: {0}", usageOptions.Cache));
                }
            })
            .ImportLogging()
            .ImportSingleton<IOptions<UsageOptions>>()
            .ForwardSingleton<IDistributedLockProvider>();

        return @this;
    }

    private static IServiceCollection AddRedisServices(this IServiceCollection @this)
    {
        @this
            .AddStackExchangeRedisExtensions<SystemTextJsonSerializer>(services =>
            {
                var redisOptions = services
                    .GetRequiredService<IOptionsSnapshot<RedisOptions>>()
                    .Value;

                return new RedisConfiguration()
                {
                    AbortOnConnectFail = true,
                    Hosts = redisOptions.Hosts.Select(e => new RedisHost() { Host = e.Address, Port = e.Port }).ToArray(),
                    Database = 0,
                    PoolSize = 16,
                    User = redisOptions.Username,
                    Password = redisOptions.Password,
                }.Yield();
            });

        return @this;
    }
}
