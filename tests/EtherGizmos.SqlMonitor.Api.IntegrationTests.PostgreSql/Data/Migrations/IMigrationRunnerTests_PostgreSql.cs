using EtherGizmos.Extensions.DependencyInjection;
using EtherGizmos.SqlMonitor.Shared.Configuration.Data;
using EtherGizmos.SqlMonitor.Shared.Database.Services;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.IntegrationTests.Data.Migrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.PostgreSql.Data.Migrations;

internal class IMigrationRunnerTests_PostgreSql : IMigrationRunnerTests
{
    public const string ConfigurationNamespace = "Connections:PostgreSql";
    public const string MigrationDatabaseName = DockerSetup.ServerDatabase + "_migration";

    public override string CreateDatabaseCommand => $"create database {MigrationDatabaseName};";

    public override string PostCreateDatabaseCommand => "create extension if not exists \"uuid-ossp\";";

    protected override void AddDatabaseConnectionProvider(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddOptions<PostgreSqlOptions>()
            .Configure<IConfiguration>((opt, conf) =>
            {
                var section = conf.GetSection(ConfigurationNamespace);

                section.Bind(opt);
                opt.AllProperties = section.GetChildren()
                    .Where(e => !typeof(PostgreSqlOptions).GetProperties().Any(p => p.Name == e.Key))
                    .ToDictionary(e => e.Key, e => e.Value);

                opt.AssertValid(ConfigurationNamespace);
            });

        serviceCollection
            .AddChildContainer((childServices, parentServices) =>
            {
                childServices.AddTransient<IDatabaseConnectionProvider, PostgreSqlDatabaseConnectionProvider>();
            })
            .ImportSingleton<IOptions<PostgreSqlOptions>>()
            .ForwardTransient<IDatabaseConnectionProvider>();
    }

    protected override void AddFluentMigratorDatabaseService(IMigrationRunnerBuilder builder)
    {
        builder.AddPostgres();
    }

    protected override IDictionary<string, string?> GetConfigurationValues()
    {
        var configOptions = new Dictionary<string, string?>()
        {
            { $"{ConfigurationNamespace}:Host", DockerSetup.ServerHost },
            { $"{ConfigurationNamespace}:Port", DockerSetup.ServerPort.ToString() },
            { $"{ConfigurationNamespace}:Database", MigrationDatabaseName },
            { $"{ConfigurationNamespace}:User Id", DockerSetup.ServerAdminUsername },
            { $"{ConfigurationNamespace}:Password", DockerSetup.ServerAdminPassword },
            { $"{ConfigurationNamespace}:Include Error Detail", "true" },
        };

        return configOptions;
    }
}
