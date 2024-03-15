using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;

/// <summary>
/// Provides access to <see cref="MonitoredEnvironment"/> records.
/// </summary>
public interface IMonitoredEnvironmentService : IEditableQueryableService<MonitoredEnvironment>
{
}
