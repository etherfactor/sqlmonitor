using EtherGizmos.SqlMonitor.Agent.Core.Helpers;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Queries.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using EtherGizmos.SqlMonitor.Shared.Utilities.Extensions;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.Diagnostics;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Queries;

/// <summary>
/// Executes queries against a MySQL database.
/// </summary>
internal class MySqlQueryRunner : IQueryRunner
{
    private readonly ILogger _logger;
    private readonly string _connectionString;

    public MySqlQueryRunner(
        ILogger<MySqlQueryRunner> logger,
        string connectionString)
    {
        _logger = logger;
        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public async Task<QueryResultMessage> ExecuteAsync(
        QueryExecuteMessage queryMessage,
        CancellationToken cancellationToken = default)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText = queryMessage.Text;

        var executedAtUtc = DateTimeOffset.UtcNow;

        //Create a stopwatch so we know how long the script took to run
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var reader = await command.ExecuteLoggedReaderAsync(_logger, cancellationToken);
        var executionMilliseconds = stopwatch.ElapsedMilliseconds;
        if (stopwatch.IsRunning && executionMilliseconds == 0)
            executionMilliseconds++;

        var result = new QueryResultMessage(
            queryMessage.QueryId,
            queryMessage.Name,
            queryMessage.MonitoredQueryTargetId,
            executionMilliseconds);

        await MessageHelper.ReadIntoMessageAsync(queryMessage, result, reader, executedAtUtc);

        return result;
    }
}
