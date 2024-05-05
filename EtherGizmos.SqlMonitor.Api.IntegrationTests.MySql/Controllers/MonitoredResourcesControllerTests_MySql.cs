using EtherGizmos.SqlMonitor.Shared.IntegrationTests.Controllers;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.MySql.Controllers;

internal class MonitoredResourcesControllerTests_MySql : MonitoredResourcesControllerTests
{
    protected override HttpClient GetClient()
    {
        return ServerSetup.GetClient();
    }
}
