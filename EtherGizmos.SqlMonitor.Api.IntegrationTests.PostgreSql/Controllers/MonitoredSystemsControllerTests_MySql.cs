using EtherGizmos.SqlMonitor.Api.IntegrationTests.PostgreSql;
using EtherGizmos.SqlMonitor.Shared.IntegrationTests.Controllers;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.PostgreSql.Controllers;

internal class MonitoredSystemsControllerTests_MySql : MonitoredSystemsControllerTests
{
    protected override HttpClient GetClient()
    {
        return ServerSetup.GetClient();
    }
}
