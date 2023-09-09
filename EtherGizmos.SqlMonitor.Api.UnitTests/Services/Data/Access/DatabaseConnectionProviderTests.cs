using EtherGizmos.SqlMonitor.Api.Services.Data;
using Microsoft.Extensions.Configuration;

namespace EtherGizmos.SqlMonitor.Api.UnitTests.Services.Data.Access;

internal class DatabaseConnectionProviderTests
{
    private DatabaseConnectionProvider? ConnectionProvider { get; set; }

    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void GetConnectionString_NotInJson_ThrowsInvalidOperationException()
    {
        var configData = new Dictionary<string, string?>()
        {
            //Intentionally empty
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        ConnectionProvider = new DatabaseConnectionProvider(config);

        Assert.Throws<InvalidOperationException>(() =>
        {
            ConnectionProvider.GetConnectionString();
        });
    }

    [Test]
    public void GetConnectionString_InJson_ReturnsConnectionString()
    {
        var configData = new Dictionary<string, string?>()
        {
            { "Connections:SqlServer:Data Source", "(mssqllocaldb)\\localhost" },
            { "Connections:SqlServer:Initial Catalog", "database" },
            { "Connections:SqlServer:Integrated Security", "true" },
            { "Connections:SqlServer:Application Name", "Unit Test" }
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        ConnectionProvider = new DatabaseConnectionProvider(config);

        var connection = ConnectionProvider.GetConnectionString();

        Assert.That(connection, Is.Not.Null);
    }
}
