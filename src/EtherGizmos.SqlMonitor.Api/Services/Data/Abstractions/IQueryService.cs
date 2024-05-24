using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;

/// <summary>
/// Provides access to <see cref="Query"/> records.
/// </summary>
public interface IQueryService : IEditableQueryableService<Query>
{
}
