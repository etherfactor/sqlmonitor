using EtherGizmos.SqlMonitor.Api.Services.Scripts;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.Ssh.Services.Scripts;

internal class SshScriptRunnerTests
{
    [Test]
    public async Task ExecuteAsync_PasswordAuthentication_ConnectsSuccessfully()
    {
        var runner = new SshScriptRunner();

        var target = new MonitoredScriptTarget()
        {
            HostName = "localhost",
            Port = 2222,
            FilePath = "/home",
            AuthenticationType = SshAuthenticationType.Password,
            Username = "root",
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

    [Test]
    public async Task ExecuteAsync_PrivateKeyAuthentication_ConnectsSuccessfully()
    {
        var runner = new SshScriptRunner();

        var target = new MonitoredScriptTarget()
        {
            HostName = "localhost",
            Port = 2222,
            FilePath = "/home",
            AuthenticationType = SshAuthenticationType.PrivateKey,
            Username = "root",
            PrivateKey = File.ReadAllText(Global.PrivateKeyFilePath),
            PrivateKeyPassword = "password",
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

    //[Test]
    //public async Task ExecuteAsync_InterpretBash_ReturnsMetrics()
    //{

    //}

    //[Test]
    //public async Task ExecuteAsync_InterpretPowerShell_ReturnsMetrics()
    //{

    //}
}
