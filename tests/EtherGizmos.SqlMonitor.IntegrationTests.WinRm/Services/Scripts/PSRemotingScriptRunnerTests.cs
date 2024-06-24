using EtherGizmos.SqlMonitor.Agent.Core.Extensions;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Pooling.Abstractions;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using EtherGizmos.SqlMonitor.Shared.Models.Communication;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using System.Management.Automation.Runspaces;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.WinRm.Services.Scripts;

internal class PSRemotingScriptRunnerTests
{
    private PSRemotingScriptRunner _runner;

    [SetUp]
    public void SetUp()
    {
        var loggerMock = new Mock<ILogger<PSRemotingScriptRunner>>();

        var config = new WinRmConfiguration()
        {
            HostName = "localhost",
            Port = 55985,
            FilePath = "C:\\",
            AuthenticationType = WinRmAuthenticationType.Basic,
            Protocol = "http",
            Username = "User",
            Password = "Password12345!",
            Command = "powershell",
            Arguments = "-File $Script",
        };

        var ticketMock = new Mock<ITicket<Runspace>>();
        ticketMock.Setup(@interface =>
            @interface.Service)
            .Returns(config.CreateRunspace());

        _runner = new PSRemotingScriptRunner(
            loggerMock.Object,
            config,
            ticketMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _runner.Dispose();
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
                Command = "powershell",
                Arguments = "-File $Script",
                Extension = "ps1",
            },
            ExecType = ExecType.WinRm,
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

        Assert.That(encoded, Is.EqualTo("$varname = 'Write-Host \"##metric value=1 bucket=`\"Test`\"\"'"));
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
