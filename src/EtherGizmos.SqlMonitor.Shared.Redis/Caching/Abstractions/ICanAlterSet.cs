namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;

public interface ICanAlterSet<TEntity>
{
    /// <summary>
    /// Adds an entity to the cache set.
    /// </summary>
    /// <param name="entity">The entity to save.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An awaitable task.</returns>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an entity from the cache set.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An awaitable task.</returns>
    Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
}
