namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface ICanAlterSet<TEntity>
{
    /// <summary>
    /// Adds an entity to the cache set.
    /// </summary>
    /// <param name="entity">The entity to save.</param>
    /// <returns>An awaitable task.</returns>
    Task AddAsync(TEntity entity);

    /// <summary>
    /// Removes an entity from the cache set.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <returns>An awaitable task.</returns>
    Task RemoveAsync(TEntity entity);
}
