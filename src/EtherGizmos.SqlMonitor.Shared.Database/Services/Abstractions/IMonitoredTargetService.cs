using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;

public interface IMonitoredTargetService : IEditableQueryableService<MonitoredTarget>
{
    Task<MonitoredTarget> GetOrCreateAsync(Guid systemId, Guid resourceId, Guid environmentId, CancellationToken cancellationToken = default);
}
