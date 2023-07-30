using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Abstractions;

/// <summary>
/// Provides access to <see cref="Query"/> records.
/// </summary>
public interface IQueryService : ICacheableQueryableService<Query>, IEditableQueryableService<Query>
{
}
