using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.SqlMonitor.Api.Controllers;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Moq;
using System.Diagnostics.CodeAnalysis;

[assembly: ExcludeFromCodeCoverage]

namespace EtherGizmos.SqlMonitor.Api.UnitTests;

internal static class Global
{
    internal static IServiceProvider CreateScope()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddMapper();

        services.AddSingleton(typeof(Mock<>));

        services.AddSingleton<ILogger>(o => o.GetRequiredService<Mock<ILogger>>().Object);
        services.AddSingleton(typeof(ILogger<>), typeof(ProxyLogger<>));

        services.AddScoped<MonitoredSystemsController>();

        services.AddSingleton<IDistributedRecordCache, InMemoryRecordCache>();
        services.AddSingleton<IRedisHelperFactory>(e => RedisHelperFactory.Instance);

        services.AddSingleton<ISaveService>(provider => provider.GetRequiredService<Mock<ISaveService>>().Object);

        services.AddSingleton<IMonitoredSystemService>(provider => provider.GetRequiredService<Mock<IMonitoredSystemService>>().Object);

        services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<Mock<ISendEndpointProvider>>().Object);

        var provider = services.BuildServiceProvider().CreateScope().ServiceProvider;
        return provider;
    }

    internal static string GetConnectionStringForMaster(this IDatabaseConnectionProvider @this)
    {
        string connectionString = @this.GetConnectionString();

        var builder = new SqlConnectionStringBuilder(connectionString);
        builder.InitialCatalog = "master";

        return builder.ConnectionString;
    }

    internal static string GetDatabaseName(this IDatabaseConnectionProvider @this)
    {
        string connectionString = @this.GetConnectionString();

        var builder = new SqlConnectionStringBuilder(connectionString);
        return builder.InitialCatalog;
    }

    internal static IEdmModel GenerateEdmModel(this ApiVersion @this)
    {
        var builder = new ODataConventionModelBuilder();

        var configurations = typeof(ApiVersions).Assembly.GetTypes()
            .Where(e => e.IsAssignableTo(typeof(IModelConfiguration)));

        foreach (var configuration in configurations)
        {
            var instance = Activator.CreateInstance(configuration) as IModelConfiguration;
            if (instance is null)
                continue;

            instance.Apply(builder, @this, "api/v{version:apiVersion}");
        }

        return builder.GetEdmModel();
    }
}
