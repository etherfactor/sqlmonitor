using EtherGizmos.Extensions.DependencyInjection;
using EtherGizmos.SqlMonitor.Shared.Configuration.Data;
using EtherGizmos.SqlMonitor.Shared.Database.Remaps;
using EtherGizmos.SqlMonitor.Shared.Database.Services;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.IntegrationTests.Data.Migrations;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.MySql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.MySql.Data.Migrations;

internal class IMigrationRunnerTests_MySql : IMigrationRunnerTests
{
    public const string ConfigurationNamespace = "Connections:MySql";
    public const string MigrationDatabaseName = DockerSetup.ServerDatabase + "_migration";

    public override string CreateDatabaseCommand => $"create database {MigrationDatabaseName};";

    protected override void AddDatabaseConnectionProvider(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddOptions<MySqlOptions>()
            .Configure<IConfiguration>((opt, conf) =>
            {
                var section = conf.GetSection(ConfigurationNamespace);

                section.Bind(opt);
                opt.AllProperties = section.GetChildren()
                    .Where(e => !typeof(MySqlOptions).GetProperties().Any(p => p.Name == e.Key))
                    .ToDictionary(e => e.Key, e => e.Value);

                opt.AssertValid(ConfigurationNamespace);
            });

        serviceCollection
            .AddChildContainer((childServices, parentServices) =>
            {
                childServices.AddTransient<IDatabaseConnectionProvider, MySqlDatabaseConnectionProvider>();
            })
            .ImportSingleton<IOptions<MySqlOptions>>()
            .ForwardTransient<IDatabaseConnectionProvider>();
    }

    protected override void AddFluentMigratorDatabaseService(IMigrationRunnerBuilder builder)
    {
        builder.AddMySql8()
            .Services.AddScoped<MySqlQuoter, MySqlQuoterRemap>();
    }

    protected override IDictionary<string, string?> GetConfigurationValues()
    {
        var configOptions = new Dictionary<string, string?>()
        {
            { $"{ConfigurationNamespace}:Server", DockerSetup.ServerHost },
            { $"{ConfigurationNamespace}:Port", DockerSetup.ServerPort.ToString() },
            { $"{ConfigurationNamespace}:Database", DockerSetup.ServerDatabase },
            { $"{ConfigurationNamespace}:Uid", DockerSetup.ServerAdminUsername },
            { $"{ConfigurationNamespace}:Pwd", DockerSetup.ServerAdminPassword },
        };

        return configOptions;
    }
}
