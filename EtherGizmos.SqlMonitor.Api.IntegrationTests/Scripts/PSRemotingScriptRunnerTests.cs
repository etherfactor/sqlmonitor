using EtherGizmos.SqlMonitor.Api.Services.Scripts;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.Scripts;

internal class PSRemotingScriptRunnerTests : IntegrationTestBase
{
    [Test]
    public async Task Test()
    {
        var runner = new PSRemotingScriptRunner();

        var target = new MonitoredScriptTarget()
        {
            HostName = "localhost",
            FilePath = "C:\\Scripts",
            UseSsl = false,
        };

        var script = new ScriptVariant()
        {
            ScriptText = "Write-Host \"##metric value=1 bucket=`\"Test`\"\"",
            ScriptInterpreter = new ScriptInterpreter()
            {
                Command = "powershell",
                Arguments = "-File $Script",
                Extension = "ps1",
            },
        };

        await runner.ExecuteAsync(target, script);
    }
}
