using EtherGizmos.SqlMonitor.Api.Controllers;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        services.AddScoped(typeof(Mock<>));

        services.AddScoped<ILogger>(o => o.GetRequiredService<Mock<ILogger>>().Object);
        services.AddScoped(typeof(ILogger<>), typeof(ProxyLogger<>));

        services.AddScoped<InstancesController>();
        services.AddScoped<PermissionsController>();
        services.AddScoped<QueriesController>();
        services.AddScoped<SecurablesController>();
        services.AddScoped<UsersController>();

        services.AddSingleton<IDistributedRecordCache, InMemoryRecordCache>();
        services.AddScoped<ISaveService>(provider => provider.GetRequiredService<Mock<ISaveService>>().Object);

        services.AddScoped<IInstanceService>(provider => provider.GetRequiredService<Mock<IInstanceService>>().Object);
        services.AddScoped<IPermissionService>(provider => provider.GetRequiredService<Mock<IPermissionService>>().Object);
        services.AddScoped<IQueryService>(provider => provider.GetRequiredService<Mock<IQueryService>>().Object);
        services.AddScoped<ISecurableService>(provider => provider.GetRequiredService<Mock<ISecurableService>>().Object);
        services.AddScoped<IUserService>(provider => provider.GetRequiredService<Mock<IUserService>>().Object);

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
}
