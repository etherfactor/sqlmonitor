﻿using EtherGizmos.SqlMonitor.Shared.Configuration.Caching;
using EtherGizmos.SqlMonitor.Shared.Utilities.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.System.Text.Json;

namespace EtherGizmos.SqlMonitor.Shared.Redis;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddRedisServices(this IServiceCollection @this)
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
