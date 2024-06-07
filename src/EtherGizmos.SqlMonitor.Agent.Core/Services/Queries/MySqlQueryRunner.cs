using EtherGizmos.SqlMonitor.Agent.Core.Helpers;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Queries.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;
using MySqlConnector;
using System.Diagnostics;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Queries;

/// <summary>
/// Executes queries against a MySQL database.
/// </summary>
public class MySqlQueryRunner : IQueryRunner
{
    private readonly string _connectionString;

    public MySqlQueryRunner(
        string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public async Task<QueryResultMessage> ExecuteAsync(
        QueryExecuteMessage queryMessage,
        CancellationToken cancellationToken = default)
    {
        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText = queryMessage.Text;

        //Create a stopwatch so we know how long the script took to run
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var reader = await command.ExecuteReaderAsync();
        var executionMilliseconds = stopwatch.ElapsedMilliseconds;
        if (stopwatch.IsRunning && executionMilliseconds == 0)
            executionMilliseconds++;

        var result = new QueryResultMessage(
            queryMessage.QueryId,
            queryMessage.Name,
            queryMessage.MonitoredQueryTargetId,
            executionMilliseconds);

        await MessageHelper.ReadIntoMessageAsync(queryMessage, result, reader);

        return result;
    }
}
