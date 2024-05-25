using EtherGizmos.SqlMonitor.Agent.Services.Queries.Abstractions;
using MySqlConnector;
using System.Diagnostics;

namespace EtherGizmos.SqlMonitor.Agent.Services.Queries;

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
    public async Task<QueryExecutionResultSet> ExecuteAsync(
        QueryVariant queryVariant,
        CancellationToken cancellationToken = default)
    {
        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText = queryVariant.QueryText;

        //Create a stopwatch so we know how long the script took to run
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var results = await command.ExecuteReaderAsync();
        var executionMilliseconds = stopwatch.ElapsedMilliseconds;
        if (stopwatch.IsRunning && executionMilliseconds == 0)
            executionMilliseconds++;

        return QueryExecutionResultSet.FromResults(queryVariant, results, executionMilliseconds);
    }
}
