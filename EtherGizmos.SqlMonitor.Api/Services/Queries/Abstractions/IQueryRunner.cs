using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Queries.Abstractions;

/// <summary>
/// Executes queries against servers, either by connecting or running them locally.
/// </summary>
public interface IQueryRunner
{
    /// <summary>
    /// Execute a query against the specified server.
    /// </summary>
    /// <param name="queryTarget">The server against which to execute the query.</param>
    /// <param name="queryVariant">The query to execute.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The query execution outputs.</returns>
    Task<QueryExecutionResultSet> ExecuteAsync(
        MonitoredQueryTarget queryTarget,
        QueryVariant queryVariant,
        CancellationToken cancellationToken = default);
}
