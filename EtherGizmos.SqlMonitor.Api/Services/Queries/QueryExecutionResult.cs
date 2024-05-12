namespace EtherGizmos.SqlMonitor.Api.Services.Queries;

/// <summary>
/// A result produced by a query.
/// </summary>
public class QueryExecutionResult
{
    /// <summary>
    /// The key-value pairs of columns and their values produced by the query.
    /// </summary>
    public required IDictionary<string, object> Values { get; set; }
}
