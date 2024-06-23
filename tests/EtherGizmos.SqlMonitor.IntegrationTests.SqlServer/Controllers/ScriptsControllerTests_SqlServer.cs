using EtherGizmos.SqlMonitor.Shared.IntegrationTests.Controllers;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.SqlServer.Controllers;

internal class ScriptsControllerTests_SqlServer : ScriptsControllerTests
{
    protected override HttpClient GetClient()
    {
        return ServerSetup.GetClient();
    }
}
