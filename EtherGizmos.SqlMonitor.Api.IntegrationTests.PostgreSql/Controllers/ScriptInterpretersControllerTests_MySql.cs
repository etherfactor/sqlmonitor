using EtherGizmos.SqlMonitor.Api.IntegrationTests.PostgreSql;
using EtherGizmos.SqlMonitor.Shared.IntegrationTests.Controllers;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.PostgreSql.Controllers;

internal class ScriptInterpretersControllerTests_MySql : ScriptInterpretersControllerTests
{
    protected override HttpClient GetClient()
    {
        return ServerSetup.GetClient();
    }
}
