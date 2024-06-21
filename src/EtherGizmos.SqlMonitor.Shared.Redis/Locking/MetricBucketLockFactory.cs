using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Locking;

internal class MetricBucketLockFactory : IMetricBucketLockFactory
{
    public string EntityType => "metric_bucket";

    public Func<MetricBucket, object?[]> KeyResolver => bucket => [bucket.Id];
}
