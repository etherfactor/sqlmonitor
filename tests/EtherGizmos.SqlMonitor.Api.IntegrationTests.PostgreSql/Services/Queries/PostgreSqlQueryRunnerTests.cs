using EtherGizmos.SqlMonitor.Agent.Core.Services.Queries;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.PostgreSql.Services.Queries;

internal class PostgreSqlQueryRunnerTests
{
    private PostgreSqlQueryRunner _runner;

    [SetUp]
    public void SetUp()
    {
        _runner = new PostgreSqlQueryRunner($"Host={DockerSetup.ServerHost}; Port={DockerSetup.ServerPort}; Database={DockerSetup.ServerDatabase}; User Id={DockerSetup.ServerDefaultUsername}; Password={DockerSetup.ServerDefaultPassword};");
    }

    [Test]
    public async Task ExecuteAsync_PasswordAuthentication_ReturnsResults()
    {
        var query = new QueryVariant()
        {
            QueryText = "select 1 as value, 'Test' as bucket;",
            SqlType = SqlType.PostgreSql,
        };

        var result = await _runner.ExecuteAsync(query);

        Assert.Multiple(() =>
        {
            Assert.That(result.Results.Count(), Is.EqualTo(1));
            Assert.That(result.QueryVariant, Is.EqualTo(query));
            Assert.That(result.ExecutionMilliseconds, Is.GreaterThan(0));

            var first = result.Results.First();
            Assert.That(first.Values["bucket"], Is.EqualTo("Test"));
            Assert.That(first.Values["value"], Is.EqualTo(1));
        });
    }
}
