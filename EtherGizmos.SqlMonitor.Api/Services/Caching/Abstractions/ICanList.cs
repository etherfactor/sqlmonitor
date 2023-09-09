namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface ICanList<TEntity>
{
    /// <summary>
    /// Fetches all entities from the cache set matching the current filter.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An awaitable task, containing the matching entities.</returns>
    Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default);
}
