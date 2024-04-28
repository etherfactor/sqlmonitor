using EtherGizmos.Extensions.DependencyInjection;
using EtherGizmos.SqlMonitor.Api.Services.Data;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Configuration;
using EtherGizmos.SqlMonitor.Database;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.Data.Migrations;

internal class IMigrationRunnerTests
{
    [Test]
    public async Task PerformMigration_ForSqlServer_MigratesAndReverts()
    {
        var databaseName = "sqlmonitor_migration";
        var configOptions = new Dictionary<string, string?>()
        {
            { "Connections:SqlServer:Data Source", "(localdb)\\mssqllocaldb" },
            { "Connections:SqlServer:Initial Catalog", databaseName },
            { "Connections:SqlServer:TrustServerCertificate", "true" },
            { "Connections:SqlServer:Integrated Security", "true" },
            { "Connections:SqlServer:Application Name", "Migration Testing" }
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configOptions)
            .Build();

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IConfiguration>(config);
        serviceCollection
            .AddOptions<SqlServerOptions>()
            .Configure<IConfiguration>((opt, conf) =>
            {
                var path = "Connections:SqlServer";

                var section = conf.GetSection(path);

                section.Bind(opt);
                opt.AllProperties = section.GetChildren()
                    .Where(e => !typeof(SqlServerOptions).GetProperties().Any(p => p.Name == e.Key))
                    .ToDictionary(e => e.Key, e => e.Value);

                opt.AssertValid(path);
            });

        serviceCollection
            .AddChildContainer((childServices, parentServices) =>
            {
                childServices.AddTransient<IDatabaseConnectionProvider, SqlServerDatabaseConnectionProvider>();
            })
            .ImportSingleton<IOptions<SqlServerOptions>>()
            .ForwardTransient<IDatabaseConnectionProvider>();

        serviceCollection
            .AddChildContainer((childServices, parentServices) =>
            {
                var connectionProvider = parentServices.GetRequiredService<IDatabaseConnectionProvider>();

                childServices.AddFluentMigratorCore()
                    .ConfigureRunner(opt =>
                    {
                        opt.AddSqlServer()
                            .WithGlobalConnectionString(connectionProvider.GetConnectionString())
                            .ScanIn(typeof(DatabaseMigrationTarget).Assembly).For.Migrations()
                            .WithVersionTable(new CustomVersionTableMetadata());
                    });
            })
            .ForwardTransient<IMigrationRunner>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var sqlOptions = serviceProvider.GetRequiredService<IOptions<SqlServerOptions>>();

        var connectionProvider = serviceProvider.GetRequiredService<IDatabaseConnectionProvider>();
        var connectionString = connectionProvider.GetConnectionStringForMaster();

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand() { Connection = connection };

        //Create a new database for this test
        command.CommandText = $"drop database if exists [{databaseName.Replace("]", "]]")}];";
        await command.ExecuteNonQueryAsync();

        command.CommandText = $"create database [{databaseName.Replace("]", "]]")}];";
        await command.ExecuteNonQueryAsync();

        //Perform the database migration
        Assert.DoesNotThrow(() =>
        {
            var migrationRunner = serviceProvider.GetRequiredService<IMigrationRunner>();

            //Migrate up to ensure forward migrations work
            migrationRunner.MigrateUp(long.MaxValue);

            //Migrate down to ensure backward migrations work
            migrationRunner.MigrateDown(long.MinValue);

            //Migrate up again to ensure the down migrations clean up everything
            migrationRunner.MigrateUp(long.MaxValue);
        });
    }
}
