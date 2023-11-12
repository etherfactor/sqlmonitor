using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;

/// <summary>
/// Provides access to <see cref="MetricBucket"/> records.
/// </summary>
public interface IMetricBucketService : IEditableQueryableService<MetricBucket>
{
}
