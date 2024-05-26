﻿using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.SqlMonitor.Api.Controllers.Api;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models;
using EtherGizmos.SqlMonitor.Shared.OData;
using EtherGizmos.SqlMonitor.Shared.Redis.Caching;
using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
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

        services.AddModelValidators();

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

    internal static IEdmModel GenerateEdmModel(this ApiVersion @this)
    {
        var builder = new ODataConventionModelBuilder();

        var configurations = typeof(ODataModelTarget).Assembly.GetTypes()
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
