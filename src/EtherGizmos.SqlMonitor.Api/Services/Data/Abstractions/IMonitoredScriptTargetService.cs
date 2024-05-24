using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;

/// <summary>
/// Provides access to <see cref="MonitoredScriptTarget"/> records.
/// </summary>
public interface IMonitoredScriptTargetService : IEditableQueryableService<MonitoredScriptTarget>
{
}
