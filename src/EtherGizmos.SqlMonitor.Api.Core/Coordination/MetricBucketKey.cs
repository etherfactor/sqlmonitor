using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.Core.Coordination;

internal class MetricBucketKey : IEntityKey<MetricBucket>
{
    public string Name => throw new NotImplementedException();
}
