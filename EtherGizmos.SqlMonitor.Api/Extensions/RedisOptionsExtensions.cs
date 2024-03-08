using EtherGizmos.SqlMonitor.Api.Services.Caching.Configuration;
using StackExchange.Redis;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

internal static class RedisOptionsExtensions
{
    public static ConfigurationOptions ToStackExchangeRedisOptions(this RedisOptions @this)
    {
        var config = new ConfigurationOptions();

        config.User = @this.Username;
        config.Password = @this.Password;

        foreach (var host in @this.Hosts)
        {
            var useAddress = host.Address;
            var usePort = host.Port != 0 ? host.Port : Constants.Redis.Port;

            config.EndPoints.Add(useAddress, usePort);
        }

        return config;
    }
}
