using EtherGizmos.SqlMonitor.Agent.Services.Queries.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;
using Npgsql;
using System.Diagnostics;

namespace EtherGizmos.SqlMonitor.Agent.Services.Queries;

/// <summary>
/// Executes queries against a PostgreSQL database.
/// </summary>
public class PostgreSqlQueryRunner : IQueryRunner
{
    private readonly string _connectionString;

    public PostgreSqlQueryRunner(
        string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public async Task<QueryExecutionResultSet> ExecuteAsync(
        QueryVariant queryVariant,
        CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
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
