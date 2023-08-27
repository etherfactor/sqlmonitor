using EtherGizmos.SqlMonitor.Api.Data.Migrations;
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
            { "Connections:Database:Data Source", "(localdb)\\mssqllocaldb" },
            { "Connections:Database:Initial Catalog", databaseName },
            { "Connections:Database:TrustServerCertificate", "true" },
            { "Connections:Database:Integrated Security", "true" },
            { "Connections:Database:Application Name", "Migration Testing" }
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(options)
            .Build();

        var connectionProvider = new DatabaseConnectionProvider(config);
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
