using EtherGizmos.SqlMonitor.Api.Services.Scripts;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.Scripts;

internal class SshScriptRunnerTests : IntegrationTestBase
{
    [Test]
    public async Task Test()
    {
        var runner = new SshScriptRunner();

        var target = new MonitoredScriptTarget()
        {
            HostName = "localhost",
            FilePath = "/scripts",
            UseSsl = false,
            Username = "username",
            Password = "password",
        };

        var script = new ScriptVariant()
        {
            ScriptText = "Write-Host \"##metric value=1 bucket=`\"Test`\"\"",
            ScriptInterpreter = new ScriptInterpreter()
            {
                Command = "pwsh",
                Arguments = "-File $Script",
                Extension = "ps1",
            },
        };

        await runner.ExecuteAsync(target, script);
    }
}
