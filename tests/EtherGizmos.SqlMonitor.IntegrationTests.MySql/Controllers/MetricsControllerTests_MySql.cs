using EtherGizmos.SqlMonitor.Shared.IntegrationTests.Controllers;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.MySql.Controllers;

internal class MetricsControllerTests_MySql : MetricsControllerTests
{
    protected override HttpClient GetClient()
    {
        return ServerSetup.GetClient();
    }
}
