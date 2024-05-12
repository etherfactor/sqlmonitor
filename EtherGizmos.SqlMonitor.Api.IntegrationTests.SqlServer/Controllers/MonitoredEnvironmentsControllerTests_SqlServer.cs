using EtherGizmos.SqlMonitor.Shared.IntegrationTests.Controllers;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.SqlServer.Controllers;

internal class MonitoredEnvironmentsControllerTests_SqlServer : MonitoredEnvironmentsControllerTests
{
    protected override HttpClient GetClient()
    {
        return ServerSetup.GetClient();
    }
}
