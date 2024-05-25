﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;

[assembly: ExcludeFromCodeCoverage]

namespace EtherGizmos.SqlMonitor.UnitTests;

internal static class Global
{
    internal static IServiceProvider CreateScope()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddMapper();

        services.AddSingleton(typeof(Mock<>));

        services.AddSingleton(o => o.GetRequiredService<Mock<ILogger>>().Object);
        services.AddSingleton(typeof(ILogger<>), typeof(ProxyLogger<>));

        services.AddScoped<MetricsController>();
        services.AddScoped<MonitoredEnvironmentsController>();
        services.AddScoped<MonitoredResourcesController>();
        services.AddScoped<MonitoredScriptTargetsController>();
        services.AddScoped<MonitoredSystemsController>();
        services.AddScoped<QueriesController>();
        services.AddScoped<ScriptsController>();
        services.AddScoped<ScriptInterpretersController>();

        services.AddSingleton<IDistributedRecordCache, InMemoryRecordCache>();
        services.AddSingleton(e => RedisHelperFactory.Instance);

        services.AddSingleton(provider => provider.GetRequiredService<Mock<ISaveService>>().Object);

        services.AddSingleton(provider => provider.GetRequiredService<Mock<IMetricService>>().Object);
        services.AddSingleton(provider => provider.GetRequiredService<Mock<IMonitoredEnvironmentService>>().Object);
        services.AddSingleton(provider => provider.GetRequiredService<Mock<IMonitoredResourceService>>().Object);
        services.AddSingleton(provider => provider.GetRequiredService<Mock<IMonitoredScriptTargetService>>().Object);
        services.AddSingleton(provider => provider.GetRequiredService<Mock<IMonitoredSystemService>>().Object);
        services.AddSingleton(provider => provider.GetRequiredService<Mock<IQueryService>>().Object);
        services.AddSingleton(provider => provider.GetRequiredService<Mock<IScriptService>>().Object);
        services.AddSingleton(provider => provider.GetRequiredService<Mock<IScriptInterpreterService>>().Object);

        services.AddSingleton(provider => provider.GetRequiredService<Mock<ISendEndpointProvider>>().Object);

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