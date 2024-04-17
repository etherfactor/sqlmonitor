using EtherGizmos.SqlMonitor.Api.Services.Scripts;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.WinRm.Services.Scripts;

internal class PSRemotingScriptRunnerTests
{
    private PSRemotingScriptRunner _runner;

    [SetUp]
    public void SetUp()
    {
        _runner = new PSRemotingScriptRunner();
    }

    [Test]
    public async Task ExecuteAsync_PasswordAuthentication_ReturnsResults()
    {
        var target = new MonitoredScriptTarget()
        {
            HostName = "localhost",
            Port = 55985,
            RunInPath = "C:\\",
            WinRmAuthenticationType = WinRmAuthenticationType.Basic,
            WinRmUseSsl = false,
            WinRmUsername = "User",
            WinRmPassword = "Password12345!",
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

        var result = await _runner.ExecuteAsync(target, script);

        Assert.Multiple(() =>
        {
            Assert.That(result.Results.Count(), Is.EqualTo(1));
            Assert.That(result.MonitoredScriptTarget, Is.EqualTo(target));
            Assert.That(result.ScriptVariant, Is.EqualTo(script));
            Assert.That(result.ExecutionMilliseconds, Is.GreaterThan(0));

            var first = result.Results.First();
            Assert.That(first.Values["bucket"], Is.EqualTo("Test"));
            Assert.That(first.Values["value"], Is.EqualTo("1"));
        });
    }
}
