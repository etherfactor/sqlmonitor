using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;

/// <summary>
/// Provides access to <see cref="MonitoredResource"/> records.
/// </summary>
public interface IMonitoredResourceService : IEditableQueryableService<MonitoredResource>
{
}
