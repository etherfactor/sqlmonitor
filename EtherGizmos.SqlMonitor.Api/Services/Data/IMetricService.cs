using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides access to <see cref="Metric"/> records.
/// </summary>
public interface IMetricService : IEditableQueryableService<Metric>
{
}
