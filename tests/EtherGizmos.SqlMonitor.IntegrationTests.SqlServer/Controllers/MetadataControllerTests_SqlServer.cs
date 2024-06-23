using EtherGizmos.SqlMonitor.Shared.IntegrationTests.Controllers;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.SqlServer.Controllers;

internal class MetadataControllerTests_SqlServer : MetadataControllerTests
{
    protected override HttpClient GetClient()
    {
        return ServerSetup.GetClient();
    }
}
