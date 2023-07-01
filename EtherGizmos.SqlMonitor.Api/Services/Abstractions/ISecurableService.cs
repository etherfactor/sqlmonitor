using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Abstractions;

/// <summary>
/// Provides access to <see cref="Securable"/> records.
/// </summary>
public interface ISecurableService : IQueryableService<Securable>
{
}
