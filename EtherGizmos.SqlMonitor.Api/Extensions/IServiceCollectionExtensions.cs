using AutoMapper;
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
                opt.AddMonitoredSystem();
                opt.AddQueryInstance();
                opt.AddQueryInstanceDatabase();
                opt.AddQueryMetric();
                opt.AddQueryMetricSeverity();
                opt.AddPermission();
                opt.AddQuery();
                opt.AddUser();
            });

            return configuration.CreateMapper();
        });

        return @this;
    }
}
