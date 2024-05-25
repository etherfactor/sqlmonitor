using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Agent.Services.Queries.Abstractions;

/// <summary>
/// Executes queries against a server, either by connecting or running them locally.
/// </summary>
public interface IQueryRunner
{
    /// <summary>
    /// Execute a query against the connected server.
    /// </summary>
    /// <param name="queryVariant">The query to execute.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The query execution outputs.</returns>
    Task<QueryExecutionResultSet> ExecuteAsync(
        QueryVariant queryVariant,
        CancellationToken cancellationToken = default);
}
