using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;

public interface IMonitoredTargetLockFactory :
    IEntityTypeLockFactory<MonitoredTarget>,
    IEntityIdLockFactory<MonitoredTarget, int>,
    IEntityIdLockFactory<MonitoredTarget, Guid, Guid, Guid>
{
}
