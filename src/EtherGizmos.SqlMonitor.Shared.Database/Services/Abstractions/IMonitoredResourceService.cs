using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;

/// <summary>
/// Provides access to <see cref="MonitoredResource"/> records.
/// </summary>
public interface IMonitoredResourceService : IEditableQueryableService<MonitoredResource>
{
}
