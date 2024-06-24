using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;

/// <summary>
/// Provides access to <see cref="Metric"/> records.
/// </summary>
public interface IMetricService : IEditableQueryableService<Metric>
{
}
