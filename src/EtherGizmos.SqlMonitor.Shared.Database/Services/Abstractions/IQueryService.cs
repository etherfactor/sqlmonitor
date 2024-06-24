using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;

/// <summary>
/// Provides access to <see cref="Query"/> records.
/// </summary>
public interface IQueryService : IEditableQueryableService<Query>
{
}
