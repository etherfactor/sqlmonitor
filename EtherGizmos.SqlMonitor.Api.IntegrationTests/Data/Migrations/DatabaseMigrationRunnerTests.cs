using EtherGizmos.SqlMonitor.Api.Services.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.Data.Migrations;

internal class DatabaseMigrationRunnerTests
{
    [Test]
    public async Task DatabaseMigratesAndReverts()
    {
        var databaseName = "sqlmonitor_migration";
        var options = new Dictionary<string, string?>()
        {
            { "Connections:SqlServer:Data Source", "(localdb)\\mssqllocaldb" },
            { "Connections:SqlServer:Initial Catalog", databaseName },
            { "Connections:SqlServer:TrustServerCertificate", "true" },
            { "Connections:SqlServer:Integrated Security", "true" },
            { "Connections:SqlServer:Application Name", "Migration Testing" }
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(options)
            .Build();

        var connectionProvider = new SqlServerDatabaseConnectionProvider(config);
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
            DatabaseMigrationRunner.PerformMigration(connectionProvider);
            DatabaseMigrationRunner.RevertMigration(connectionProvider);
        });
    }
}
