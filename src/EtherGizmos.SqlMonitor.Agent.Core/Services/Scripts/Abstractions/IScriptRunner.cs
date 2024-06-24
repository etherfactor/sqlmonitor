using EtherGizmos.SqlMonitor.Shared.Messaging.Messages;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts.Abstractions;

/// <summary>
/// Executes scripts against servers, either by connecting or running them locally.
/// </summary>
public interface IScriptRunner : IDisposable
{
    /// <summary>
    /// Execute a script against the specified server.
    /// </summary>
    /// <param name="scriptMessage">The script to execute.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The script execution outputs.</returns>
    Task<ScriptResultMessage> ExecuteAsync(
        ScriptExecuteMessage scriptMessage,
        CancellationToken cancellationToken = default);
}
