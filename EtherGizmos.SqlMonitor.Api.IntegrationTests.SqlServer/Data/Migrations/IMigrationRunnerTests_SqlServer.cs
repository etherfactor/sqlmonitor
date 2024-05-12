using EtherGizmos.Extensions.DependencyInjection;
using EtherGizmos.SqlMonitor.Api.Services.Data;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Configuration;
using EtherGizmos.SqlMonitor.Shared.IntegrationTests.Data.Migrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.SqlServer.Data.Migrations;

internal class IMigrationRunnerTests_SqlServer : IMigrationRunnerTests
{
    public const string ConfigurationNamespace = "Connections:SqlServer";
    public const string MigrationDatabaseName = DockerSetup.ServerDatabase + "_migration";

    public override string CreateDatabaseCommand => $"create database {MigrationDatabaseName}";

    protected override void AddDatabaseConnectionProvider(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddOptions<SqlServerOptions>()
            .Configure<IConfiguration>((opt, conf) =>
            {
                var section = conf.GetSection(ConfigurationNamespace);

                section.Bind(opt);
                opt.AllProperties = section.GetChildren()
                    .Where(e => !typeof(SqlServerOptions).GetProperties().Any(p => p.Name == e.Key))
                    .ToDictionary(e => e.Key, e => e.Value);

                opt.AssertValid(ConfigurationNamespace);
            });

        serviceCollection
            .AddChildContainer((childServices, parentServices) =>
            {
                childServices.AddTransient<IDatabaseConnectionProvider, SqlServerDatabaseConnectionProvider>();
            })
            .ImportSingleton<IOptions<SqlServerOptions>>()
            .ForwardTransient<IDatabaseConnectionProvider>();
    }

    protected override void AddFluentMigratorDatabaseService(IMigrationRunnerBuilder builder)
    {
        builder.AddSqlServer();
    }

    protected override IDictionary<string, string?> GetConfigurationValues()
    {
        var configOptions = new Dictionary<string, string?>()
        {
            { $"{ConfigurationNamespace}:Data Source", $"{DockerSetup.ServerHost},{DockerSetup.ServerPort}" },
            { $"{ConfigurationNamespace}:Initial Catalog", MigrationDatabaseName },
            { $"{ConfigurationNamespace}:TrustServerCertificate", "true" },
            { $"{ConfigurationNamespace}:User Id", DockerSetup.ServerAdminUsername },
            { $"{ConfigurationNamespace}:Password", DockerSetup.ServerAdminPassword },
            { $"{ConfigurationNamespace}:Application Name", "Migration Testing" }
        };

        return configOptions;
    }
}
