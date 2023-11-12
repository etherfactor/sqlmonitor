namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface ICanGetFromSet<TEntity>
{
    /// <summary>
    /// Retrieves an entity from the cache set.
    /// </summary>
    /// <param name="keys">The keys of the entity being fetched. Note that these must be in lexicographical order.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The entity, if it exists.</returns>
    Task<TEntity?> GetAsync(object[] keys, CancellationToken cancellationToken = default);
}
