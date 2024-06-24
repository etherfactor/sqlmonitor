namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;

public interface ICanGet<TEntity>
{
    /// <summary>
    /// Retrieves the entity from the cache.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The entity, if it exists.</returns>
    Task<TEntity?> GetAsync(CancellationToken cancellationToken = default);
}
