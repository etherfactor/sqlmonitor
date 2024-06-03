using EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.Ssh.Services.Scripts;

internal class SshScriptRunnerTests
{
    private SshScriptRunner _runner;

    [SetUp]
    public void SetUp()
    {
        _runner = new SshScriptRunner();
    }

    [Test]
    public async Task ExecuteAsync_PasswordAuthentication_ReturnsResults()
    {
        var target = new MonitoredScriptTarget()
        {
            HostName = "localhost",
            Port = 2222,
            RunInPath = "/home",
            SshAuthenticationType = SshAuthenticationType.Password,
            SshUsername = "root",
            SshPassword = "password",
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

    [Test]
    public async Task ExecuteAsync_PrivateKeyAuthentication_ReturnsResults()
    {
        var target = new MonitoredScriptTarget()
        {
            HostName = "localhost",
            Port = 2222,
            RunInPath = "/home",
            SshAuthenticationType = SshAuthenticationType.PrivateKey,
            SshUsername = "root",
            SshPrivateKey = File.ReadAllText(DockerSetup.PrivateKeyFilePath),
            SshPrivateKeyPassword = "password",
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

    [Test]
    public void GetSshConfiguration_ReturnsValidConfiguration()
    {
        var target = new MonitoredScriptTarget()
        {
            HostName = "localhost",
            Port = 2222,
            RunInPath = "/home",
            SshAuthenticationType = SshAuthenticationType.Password,
            SshUsername = "root",
            SshPassword = "password",
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

        var configuration = _runner.GetSshConfiguration(target, script);

        Assert.Multiple(() =>
        {
            Assert.That(configuration.HostName, Is.EqualTo(target.HostName));
            Assert.That(configuration.Port, Is.EqualTo(target.Port));
            Assert.That(configuration.FilePath, Is.EqualTo(target.RunInPath));
            Assert.That(configuration.AuthenticationType, Is.EqualTo(target.SshAuthenticationType));
            Assert.That(configuration.Username, Is.EqualTo(target.SshUsername));
            Assert.That(configuration.Password, Is.EqualTo(target.SshPassword));
            Assert.That(configuration.Command, Is.EqualTo(script.ScriptInterpreter.Command));
            Assert.That(configuration.Arguments, Is.EqualTo(script.ScriptInterpreter.Arguments));
        });
    }

    [Test]
    public void EncodeScriptToVariable_ReturnsEncodedScript()
    {
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

        var encoded = _runner.EncodeScriptToVariable("varname", script.ScriptText);

        Assert.That(encoded, Is.EqualTo("varname=$(cat << 'EOF'\nWrite-Host \"##metric value=1 bucket=`\"Test`\"\"\nEOF\n)"));
    }

    [Test]
    public void CalculateMd5Checksum_ReturnsValidChecksum()
    {
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

        var checksum = _runner.CalculateMd5Checksum(script.ScriptText);

        Assert.That(checksum, Is.EqualTo("293e9ac33bff70c8937764cbe84f7c1f"));
    }
}
