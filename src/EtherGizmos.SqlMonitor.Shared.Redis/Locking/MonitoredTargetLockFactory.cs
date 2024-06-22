using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Locking;

internal class MonitoredTargetLockFactory : IMonitoredTargetLockFactory
{
    public string EntityType => "monitored_target";

    public Func<MonitoredTarget, object?[]> KeyResolver => target => [target.Id];
}
