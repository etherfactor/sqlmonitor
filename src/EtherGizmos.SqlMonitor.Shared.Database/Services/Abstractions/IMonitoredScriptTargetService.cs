using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;

/// <summary>
/// Provides access to <see cref="MonitoredScriptTarget"/> records.
/// </summary>
public interface IMonitoredScriptTargetService : IEditableQueryableService<MonitoredScriptTarget>
{
}
