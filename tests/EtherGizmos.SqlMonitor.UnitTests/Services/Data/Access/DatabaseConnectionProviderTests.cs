using EtherGizmos.SqlMonitor.Api.Services.Data;
using EtherGizmos.SqlMonitor.Configuration.Data;
using Microsoft.Extensions.Options;
using Moq;

namespace EtherGizmos.SqlMonitor.UnitTests.Services.Data.Access;

internal class DatabaseConnectionProviderTests
{
    private SqlServerDatabaseConnectionProvider? ConnectionProvider { get; set; }

    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void GetConnectionString_InJson_ReturnsConnectionString()
    {
        var configData = new Dictionary<string, string?>()
        {
            { "Data Source", "(mssqllocaldb)\\localhost" },
            { "Initial Catalog", "database" },
            { "Integrated Security", "true" },
            { "Application Name", "Unit Test" }
        };

        var optionsMock = new Mock<IOptions<SqlServerOptions>>();
        optionsMock.Setup(@interface =>
            @interface.Value)
            .Returns(new SqlServerOptions()
            {
                AllProperties = configData
            });

        var options = optionsMock.Object;

        ConnectionProvider = new SqlServerDatabaseConnectionProvider(options);

        var connection = ConnectionProvider.GetConnectionString();

        Assert.That(connection, Is.Not.Null);
    }
}
