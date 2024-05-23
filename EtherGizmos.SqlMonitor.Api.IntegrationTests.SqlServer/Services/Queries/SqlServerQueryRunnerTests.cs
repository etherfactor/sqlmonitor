using EtherGizmos.SqlMonitor.Agent.Services.Queries;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.SqlServer.Services.Queries;

internal class SqlServerQueryRunnerTests
{
    private SqlServerQueryRunner _runner;

    [SetUp]
    public void SetUp()
    {
        _runner = new SqlServerQueryRunner("Server=localhost,11433; Database=performance_pulse; User Id=service; Password=LO^9ZpGB8FiA*HMMQyfN; TrustServerCertificate=true;");
    }

    [Test]
    public async Task ExecuteAsync_PasswordAuthentication_ReturnsResults()
    {
        var query = new QueryVariant()
        {
            QueryText = "select 1 as value, 'Test' as bucket;",
            SqlType = SqlType.MicrosoftSqlServer,
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
