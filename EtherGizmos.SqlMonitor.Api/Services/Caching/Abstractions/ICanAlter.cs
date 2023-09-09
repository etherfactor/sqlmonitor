namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface ICanAlter<TEntity>
{
    /// <summary>
    /// Saves the entity to the cache.
    /// </summary>
    /// <param name="entity">The entity to save.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An awaitable task.</returns>
    Task SetAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the entity from the cache.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An awaitable task.</returns>
    Task DeleteAsync(CancellationToken cancellationToken = default);
}
