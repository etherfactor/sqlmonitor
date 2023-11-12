using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;

/// <summary>
/// Provides access to <see cref="Metric"/> records.
/// </summary>
public interface IMetricService : IEditableQueryableService<Metric>
{
}
