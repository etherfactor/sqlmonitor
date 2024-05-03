using EtherGizmos.SqlMonitor.Models.Database;
using System.Data.Common;

namespace EtherGizmos.SqlMonitor.Api.Services.Queries;

public class QueryExecutionResultSet
{
    public required MonitoredQueryTarget MonitoredQueryTarget { get; set; }

    public required QueryVariant QueryVariant { get; set; }

    public required long ExecutionMilliseconds { get; set; }

    public required IEnumerable<QueryExecutionResult> Results { get; set; }

    public static QueryExecutionResultSet FromResults(
        MonitoredQueryTarget queryTarget,
        QueryVariant queryVariant,
        DbDataReader reader,
        long executionMilliseconds)
    {
        var executionResults = new List<QueryExecutionResult>();

        var count = reader.FieldCount;
        while (reader.Read())
        {
            var executionResult = new QueryExecutionResult() { Values = new Dictionary<string, object>() };

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
            MonitoredQueryTarget = queryTarget,
            QueryVariant = queryVariant,
            ExecutionMilliseconds = executionMilliseconds,
            Results = executionResults,
        };
    }
}
