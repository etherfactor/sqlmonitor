using EtherGizmos.SqlMonitor.Api.Services;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests;

[SetUpFixture]
internal static class Global
{
    private static WebApplicationFactory<Program>? Factory { get; set; }

    private static string TestDatabaseName { get; set; }

    private static IDatabaseConnectionProvider ConnectionProvider { get; set; }

    internal static HttpClient GetClient()
    {
        Factory ??= GetFactory();
        return Factory.CreateDefaultClient();
    }

    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        //Load app settings
        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json")
            .AddJsonFile("appsettings.Integration.json");

        var config = configBuilder.Build();

        ConnectionProvider = new DatabaseConnectionProvider(config);

        //Connection string for master will be used to create a new database for each test
        string connectionString = ConnectionProvider.GetConnectionStringForMaster();
        string baseDatabaseName = ConnectionProvider.GetDatabaseName();

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand() { Connection = connection };

        //Create a unique database name for this set of tests
        TestDatabaseName = $"{baseDatabaseName}_{DateTime.Now:yyyyMMddHHmmss}";

        command.CommandText = $"create database [{TestDatabaseName.Replace("]", "]]")}];";
        await command.ExecuteNonQueryAsync();
    }

    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        string connectionString = ConnectionProvider.GetConnectionStringForMaster();

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand() { Connection = connection };

        //Drop the database that was created for these tests
        command.CommandText = $"alter database [{TestDatabaseName.Replace("]", "]]")}] set single_user with rollback immediate;";
        await command.ExecuteNonQueryAsync();

        command.CommandText = $"drop database [{TestDatabaseName.Replace("]", "]]")}];";
        await connection.CloseAsync();
    }

    private static WebApplicationFactory<Program> GetFactory()
    {
        string projectDirectory = Directory.GetCurrentDirectory();

        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile(Path.Combine(projectDirectory, "appsettings.Integration.json"));

                //Override the database name for the connection string
                config.AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { "Connections:Database:Initial Catalog", TestDatabaseName }
                });
            });
        });

        return factory;
    }

    private static string GetConnectionStringForMaster(this IDatabaseConnectionProvider @this)
    {
        string connectionString = @this.GetConnectionString();

        var builder = new SqlConnectionStringBuilder(connectionString);
        builder.InitialCatalog = "master";

        return builder.ConnectionString;
    }

    private static string GetDatabaseName(this IDatabaseConnectionProvider @this)
    {
        string connectionString = @this.GetConnectionString();

        var builder = new SqlConnectionStringBuilder(connectionString);
        return builder.InitialCatalog;
    }
}
