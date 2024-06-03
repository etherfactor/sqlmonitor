using EtherGizmos.SqlMonitor.Agent.Core.Services.Queries;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.MySql.Services.Queries;

internal class MySqlQueryRunnerTests
{
    private MySqlQueryRunner _runner;

    [SetUp]
    public void SetUp()
    {
        _runner = new MySqlQueryRunner("Server=localhost; Port=33306; Database=performance_pulse; Uid=service; Pwd=jipEZk@7ui2lw&XUiw^W;");
    }

    [Test]
    public async Task ExecuteAsync_PasswordAuthentication_ReturnsResults()
    {
        var query = new QueryVariant()
        {
            QueryText = "select 1 as value, 'Test' as bucket from dual;",
            SqlType = SqlType.MySql,
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
