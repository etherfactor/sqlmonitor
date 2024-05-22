using EtherGizmos.SqlMonitor.Models.Database;
using System.Data.Common;

namespace EtherGizmos.SqlMonitor.Agent.Services.Queries;

/// <summary>
/// A set of results produced by a query.
/// </summary>
public class QueryExecutionResultSet
{
    /// <summary>
    /// The query that was run.
    /// </summary>
    public required QueryVariant QueryVariant { get; set; }

    /// <summary>
    /// The duration the query took to run.
    /// </summary>
    public required long ExecutionMilliseconds { get; set; }

    /// <summary>
    /// The column-value pairs returned by the query.
    /// </summary>
    public required IEnumerable<QueryExecutionResult> Results { get; set; }

    /// <summary>
    /// Create a result set from a <see cref="DbDataReader"/>.
    /// </summary>
    /// <param name="queryVariant">The query that was run.</param>
    /// <param name="reader">The reader produced by running the query.</param>
    /// <param name="executionMilliseconds">The duration the query took to run.</param>
    /// <returns>The set of results produced by the query.</returns>
    public static QueryExecutionResultSet FromResults(
        QueryVariant queryVariant,
        DbDataReader reader,
        long executionMilliseconds)
    {
        var executionResults = new List<QueryExecutionResult>();

        var count = reader.FieldCount;
        while (reader.Read())
        {
            var executionResult = new QueryExecutionResult() { Values = new Dictionary<string, object>() };

            //Place each column as a key-value pair in the result
            for (int i = 0; i < count; i++)
            {
                var variableKey = reader.GetName(i);
                var variableValue = reader.GetValue(i);

                executionResult.Values.TryAdd(variableKey, variableValue);
            }

            executionResults.Add(executionResult);
        }

        return new()
        {
            QueryVariant = queryVariant,
            ExecutionMilliseconds = executionMilliseconds,
            Results = executionResults,
        };
    }
}
