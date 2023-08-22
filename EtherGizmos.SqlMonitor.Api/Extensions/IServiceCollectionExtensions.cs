using AutoMapper;
using EtherGizmos.SqlMonitor.Api.Jobs;
using EtherGizmos.SqlMonitor.Api.Jobs.Abstractions;
using EtherGizmos.SqlMonitor.Models.Api.v1;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
}
