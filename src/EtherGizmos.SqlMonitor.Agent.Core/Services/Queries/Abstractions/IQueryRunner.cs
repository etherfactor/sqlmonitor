using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Queries.Abstractions;

/// <summary>
/// Executes queries against a server, either by connecting or running them locally.
/// </summary>
public interface IQueryRunner
{
    /// <summary>
    /// Execute a query against the connected server.
    /// </summary>
    /// <param name="queryMessage">The query to execute.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The query execution outputs.</returns>
    Task<QueryResultMessage> ExecuteAsync(
        QueryExecuteMessage queryMessage,
        CancellationToken cancellationToken = default);
}
