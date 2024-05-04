using EtherGizmos.Extensions.DependencyInjection;
using EtherGizmos.SqlMonitor.Api.Services.Data;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.SqlServer;

[SetUpFixture]
internal static class ServerSetup
{
    private static WebApplicationFactory<Program>? Factory { get; set; }

    private static string TestDatabaseName { get; set; }

    private static IDatabaseConnectionProvider ConnectionProvider { get; set; }

    internal static HttpClient GetClient()
    {
        if (Factory is null)
            throw new InvalidOperationException("Web application factory is not yet initialized.");

        return Factory.CreateDefaultClient();
    }

    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        Factory = GetFactory();

        //Load app settings
        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json")
            .AddJsonFile("appsettings.Integration.json");

        var config = configBuilder.Build();

        //Build settings into a service collection
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

        var serviceProvider = serviceCollection.BuildServiceProvider();

        ConnectionProvider = serviceProvider.GetRequiredService<IDatabaseConnectionProvider>();

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
        await command.ExecuteNonQueryAsync();

        Factory?.Dispose();
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
                config.AddInMemoryCollection(new Dictionary<string, string?>()
                {
                    { "Connections:SqlServer:Initial Catalog", TestDatabaseName }
                });
            });
        });

        return factory;
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
