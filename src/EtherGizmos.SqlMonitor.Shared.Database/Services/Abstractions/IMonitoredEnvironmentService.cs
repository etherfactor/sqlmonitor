using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;

/// <summary>
/// Provides access to <see cref="MonitoredEnvironment"/> records.
/// </summary>
public interface IMonitoredEnvironmentService : IEditableQueryableService<MonitoredEnvironment>
{
}
