﻿using EtherGizmos.SqlMonitor.Agent.Core.Services.Pooling.Abstractions;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Queries;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.PostgreSql.Services.Queries;

internal class PostgreSqlQueryRunnerTests
{
    private PostgreSqlQueryRunner _runner;

    [SetUp]
    public void SetUp()
    {
        var loggerMock = new Mock<ILogger<PostgreSqlQueryRunner>>();

        var ticketMock = new Mock<ITicket<NpgsqlConnection>>();
        ticketMock.Setup(@interface =>
            @interface.Service)
            .Returns(() =>
            {
                var connection = new NpgsqlConnection($"Host={DockerSetup.ServerHost}; Port={DockerSetup.ServerPort}; Database={DockerSetup.ServerDatabase}; User Id={DockerSetup.ServerDefaultUsername}; Password={DockerSetup.ServerDefaultPassword};");
                connection.Open();
                return connection;
            });

        _runner = new PostgreSqlQueryRunner(
            loggerMock.Object,
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
        var query = new QueryExecuteMessage()
        {
            QueryId = Guid.NewGuid(),
            Name = "Test Query",
            MonitoredQueryTargetId = 1,
            ConnectionRequestToken = "blah",
            SqlType = SqlType.PostgreSql,
            Text = "select 1 as value, 'Test' as bucket;",
            BucketColumn = "bucket",
            TimestampUtcColumn = null,
            Metrics =
            [
                new()
                {
                    MetricId = 1,
                    ValueColumn = "value",
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
