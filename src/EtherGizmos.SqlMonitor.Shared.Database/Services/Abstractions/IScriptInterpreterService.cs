using EtherGizmos.SqlMonitor.Shared.Models.Database;
namespace EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;

/// <summary>
/// Provides access to <see cref="ScriptInterpreter"/> records.
/// </summary>
public interface IScriptInterpreterService : IEditableQueryableService<ScriptInterpreter>
{
}
