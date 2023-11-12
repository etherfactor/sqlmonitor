using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;

/// <summary>
/// Provides access to <see cref="InstanceMetricBySecond"/> records.
/// </summary>
public interface IInstanceMetricBySecondService : IEditableQueryableService<InstanceMetricBySecond>
{
}
