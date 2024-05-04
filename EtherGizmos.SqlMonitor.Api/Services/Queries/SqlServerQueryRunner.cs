using EtherGizmos.SqlMonitor.Models.Database;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace EtherGizmos.SqlMonitor.Api.Services.Queries;

public class SqlServerQueryRunner : IQueryRunner
{
    /// <inheritdoc/>
    public async Task<QueryExecutionResultSet> ExecuteAsync(
        MonitoredQueryTarget queryTarget,
        QueryVariant queryVariant,
        CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(queryTarget.ConnectionString);
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

        return QueryExecutionResultSet.FromResults(queryTarget, queryVariant, results, executionMilliseconds);
    }
}
