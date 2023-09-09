namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface ICanAlter<TEntity>
{
    /// <summary>
    /// Saves the entity to the cache.
    /// </summary>
    /// <param name="entity">The entity to save.</param>
    /// <returns>An awaitable task.</returns>
    Task SetAsync(TEntity entity);

    /// <summary>
    /// Deletes the entity from the cache.
    /// </summary>
    /// <returns>An awaitable task.</returns>
    Task DeleteAsync();
}
