using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;

/// <summary>
/// Provides access to <see cref="MonitoredSystem"/> records.
/// </summary>
public interface IMonitoredSystemService : IEditableQueryableService<MonitoredSystem>
{
}
