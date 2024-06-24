using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;

/// <summary>
/// Provides access to <see cref="MonitoredQueryTarget"/> records.
/// </summary>
public interface IMonitoredQueryTargetService : IEditableQueryableService<MonitoredQueryTarget>
{
}
