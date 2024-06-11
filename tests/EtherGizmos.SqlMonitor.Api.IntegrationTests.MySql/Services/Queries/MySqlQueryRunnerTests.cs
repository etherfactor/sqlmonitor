using EtherGizmos.SqlMonitor.Agent.Core.Services.Queries;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.MySql.Services.Queries;

internal class MySqlQueryRunnerTests
{
    private MySqlQueryRunner _runner;

    [SetUp]
    public void SetUp()
    {
        var loggerMock = new Mock<ILogger<MySqlQueryRunner>>();

        _runner = new MySqlQueryRunner(
            loggerMock.Object,
            "Server=localhost; Port=33306; Database=performance_pulse; Uid=service; Pwd=jipEZk@7ui2lw&XUiw^W;");
    }

    [Test]
    public async Task ExecuteAsync_PasswordAuthentication_ReturnsResults()
    {
        var query = new QueryExecuteMessage()
        {
            QueryId = Guid.NewGuid(),
            Name = "Test Query",
            MonitoredQueryTargetId = 1,
            ConnectionRequestToken = "blah",
            SqlType = SqlType.MySql,
            Text = "select 1 as value, 'Test' as bucket from dual;",
            BucketColumn = "bucket",
            TimestampUtcColumn = null,
            Metrics =
            [
                new()
                {
                    MetricId = 1,
                    ValueColumn = "value"
                },
            ],
        };

        var result = await _runner.ExecuteAsync(query);

        Assert.Multiple(() =>
        {
            Assert.That(result.MetricValues.Count(), Is.EqualTo(1));
            Assert.That(result.QueryId, Is.EqualTo(query.QueryId));
            Assert.That(result.ExecutionMilliseconds, Is.GreaterThan(0));

            var first = result.MetricValues.First();
            Assert.That(first.Bucket, Is.EqualTo("Test"));
            Assert.That(first.Value, Is.EqualTo(1));
        });
    }
}
