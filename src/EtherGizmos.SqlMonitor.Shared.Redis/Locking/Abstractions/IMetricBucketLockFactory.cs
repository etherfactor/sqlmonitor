using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;

public interface IMetricBucketLockFactory :
    IEntityTypeLockFactory<MetricBucket>,
    IEntityIdLockFactory<MetricBucket, int>,
    IEntityIdLockFactory<MetricBucket, string>
{
}
