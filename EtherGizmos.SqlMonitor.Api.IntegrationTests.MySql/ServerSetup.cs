using EtherGizmos.SqlMonitor.Shared.IntegrationTests;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.MySql;

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
            { "Connections:Use:Database", "MySql" },
            { "Connections:Use:Cache", "InMemory" },
            { "Connections:Use:MessageBroker", "InMemory" },
            { "Connections:MySql:Server", DockerSetup.ServerHost },
            { "Connections:MySql:Port", DockerSetup.ServerPort.ToString() },
            { "Connections:MySql:Uid", DockerSetup.ServerDefaultUsername },
            { "Connections:MySql:Pwd", DockerSetup.ServerDefaultPassword },
        };

        return configValues;
    }
}
