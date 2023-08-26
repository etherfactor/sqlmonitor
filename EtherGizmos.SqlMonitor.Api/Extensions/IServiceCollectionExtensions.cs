using AutoMapper;
using EtherGizmos.SqlMonitor.Api.Data.Access;
using EtherGizmos.SqlMonitor.Api.Jobs;
using EtherGizmos.SqlMonitor.Api.Jobs.Abstractions;
using EtherGizmos.SqlMonitor.Models.Api.v1;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Configuration;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddHangfireJob<TInterface, TImplementation>(this IServiceCollection @this)
        where TInterface : class, IJob
        where TImplementation : class, TInterface
    {
        @this.TryAddScoped(typeof(IHangfireRepeatedJob<>), typeof(HangfireRepeatedJob<>));

        @this.AddScoped<TInterface, TImplementation>();

        return @this;
    }

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

    public static IServiceCollection AddRedisCache(this IServiceCollection @this, IConfigurationSection section)
    {
        var options = new ConfigurationOptions();
        section.Bind(options);

        var endpoints = section.GetSection("EndPoints").Get<RedisHost[]>()
            ?? Array.Empty<RedisHost>();
        foreach (var endpoint in endpoints)
        {
            options.EndPoints.Add(endpoint.Host, endpoint.Port);
        }

        RedisConnectionMultiplexer.Initialize(options);
        var multiplexer = RedisConnectionMultiplexer.Instance;
        @this.AddSingleton(e => multiplexer);

        @this.AddStackExchangeRedisCache(opt =>
        {
            opt.ConfigurationOptions = options;
            opt.ConnectionMultiplexerFactory = () => Task.FromResult(multiplexer);
        });

        return @this;
    }
}
