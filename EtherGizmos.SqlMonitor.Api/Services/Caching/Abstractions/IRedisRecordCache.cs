namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

/// <summary>
/// Provides means to cache and retrieve records and record sets in Redis.
/// </summary>
public interface IRedisRecordCache
{
    /// <summary>
    /// Provides means to cache and retrieve records in Redis.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being stored.</typeparam>
    /// <param name="key">The key indicating where in the cache the entity is stored.</param>
    /// <returns>The cache entity helper.</returns>
    ICacheEntity<TEntity> Entity<TEntity>(EntityCacheKey<TEntity> key)
        where TEntity : new();

    /// <summary>
    /// Provides means to cache and retrieve record sets in Redis.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being stored.</typeparam>
    /// <returns>The cache entity set helper.</returns>
    ICacheEntitySet<TEntity> EntitySet<TEntity>()
        where TEntity : new();
}
