using EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using EtherGizmos.SqlMonitor.Shared.Models.Communication;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.Ssh.Services.Scripts;

internal class SshScriptRunnerTests
{
    private SshScriptRunner _runner;

    [SetUp]
    public void SetUp()
    {
        var config = new SshConfiguration()
        {
            HostName = "localhost",
            Port = 2222,
            FilePath = "/home",
            AuthenticationType = SshAuthenticationType.Password,
            Username = "root",
            Password = "password",
            Command = "pwsh",
            Arguments = "-File $Script",
        };

        _runner = new SshScriptRunner(config);
    }

    [Test]
    public async Task ExecuteAsync_PasswordAuthentication_ReturnsResults()
    {
        var script = new ScriptExecuteMessage()
        {
            ScriptId = Guid.NewGuid(),
            Name = "Test Script",
            MonitoredScriptTargetId = 1,
            Interpreter = new()
            {
                Command = "pwsh",
                Arguments = "-File $Script",
                Extension = "ps1",
            },
            ExecType = ExecType.Ssh,
            Text = "Write-Host \"##metric value=1 bucket=`\"Test`\"\"",
            BucketKey = "bucket",
            TimestampUtcKey = null,
            Metrics =
            [
                new()
                {
                    MetricId = 1,
                    ValueKey = "value",
                },
            ],
        };

        var result = await _runner.ExecuteAsync(script);

        Assert.Multiple(() =>
        {
            Assert.That(result.MetricValues.Count(), Is.EqualTo(1));
            Assert.That(result.ScriptId, Is.EqualTo(script.ScriptId));
            Assert.That(result.ExecutionMilliseconds, Is.GreaterThan(0));

            var first = result.MetricValues.First();
            Assert.That(first.Bucket, Is.EqualTo("Test"));
            Assert.That(first.Value, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task ExecuteAsync_PrivateKeyAuthentication_ReturnsResults()
    {
        var script = new ScriptExecuteMessage()
        {
            ScriptId = Guid.NewGuid(),
            Name = "Test Script",
            MonitoredScriptTargetId = 1,
            Interpreter = new ScriptExecuteMessageInterpreter()
            {
                Command = "pwsh",
                Arguments = "-File $Script",
                Extension = "ps1",
            },
            ExecType = ExecType.Ssh,
            Text = "Write-Host \"##metric value=1 bucket=`\"Test`\"\"",
            BucketKey = "bucket",
            TimestampUtcKey = null,
            Metrics =
            [
                new()
                {
                    MetricId = 1,
                    ValueKey = "value",
                },
            ],
        };

        var result = await _runner.ExecuteAsync(script);

        Assert.Multiple(() =>
        {
            Assert.That(result.MetricValues.Count(), Is.EqualTo(1));
            Assert.That(result.ScriptId, Is.EqualTo(script.ScriptId));
            Assert.That(result.ExecutionMilliseconds, Is.GreaterThan(0));

            var first = result.MetricValues.First();
            Assert.That(first.Bucket, Is.EqualTo("Test"));
            Assert.That(first.Value, Is.EqualTo(1));
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
