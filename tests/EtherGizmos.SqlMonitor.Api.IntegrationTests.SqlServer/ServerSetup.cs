using EtherGizmos.SqlMonitor.Shared.IntegrationTests;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.SqlServer;

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
            { "Connections:Use:Database", "SqlServer" },
            { "Connections:Use:Cache", "InMemory" },
            { "Connections:Use:MessageBroker", "InMemory" },
            { "Connections:SqlServer:Data Source", $"{DockerSetup.ServerHost},{DockerSetup.ServerPort}" },
            { "Connections:SqlServer:Initial Catalog", DockerSetup.ServerDatabase },
            { "Connections:SqlServer:Integrated Security", "false" },
            { "Connections:SqlServer:User Id", DockerSetup.ServerDefaultUsername },
            { "Connections:SqlServer:Password", DockerSetup.ServerDefaultPassword },
            { "Connections:SqlServer:TrustServerCertificate", "true" },
        };

        return configValues;
    }
}
