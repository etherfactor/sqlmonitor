using EtherGizmos.SqlMonitor.Shared.IntegrationTests;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.PostgreSql;

[SetUpFixture]
internal class ServerSetup : ServerSetupBase
{
    private static ServerSetup _self;

    public static HttpClient GetClient()
    {
        return _self.InternalGetClient();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _self = this;
    }

    protected override IDictionary<string, string?> GetConfigurationValues()
    {
        var configValues = new Dictionary<string, string?>()
        {
            { "Connections:Use:Database", "PostgreSql" },
            { "Connections:Use:Cache", "InMemory" },
            { "Connections:Use:MessageBroker", "InMemory" },
            { "Connections:PostgreSql:Host", DockerSetup.ServerHost },
            { "Connections:PostgreSql:Port", DockerSetup.ServerPort.ToString() },
            { "Connections:PostgreSql:Database", DockerSetup.ServerDatabase },
            { "Connections:PostgreSql:User Id", DockerSetup.ServerDefaultUsername },
            { "Connections:PostgreSql:Password", DockerSetup.ServerDefaultPassword },
        };

        return configValues;
    }
}
