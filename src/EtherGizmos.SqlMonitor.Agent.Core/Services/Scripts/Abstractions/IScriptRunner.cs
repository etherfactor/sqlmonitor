using EtherGizmos.SqlMonitor.Api.Services.Scripts;
using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts.Abstractions;

/// <summary>
/// Executes scripts against servers, either by connecting or running them locally.
/// </summary>
public interface IScriptRunner
{
    /// <summary>
    /// Execute a script against the specified server.
    /// </summary>
    /// <param name="scriptTarget">The server against which to execute the script.</param>
    /// <param name="scriptVariant">The script to execute.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The script execution outputs.</returns>
    Task<ScriptExecutionResultSet> ExecuteAsync(
        MonitoredScriptTarget scriptTarget,
        ScriptVariant scriptVariant,
        CancellationToken cancellationToken = default);
}
