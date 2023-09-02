namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface ILockedDistributedCache
{
    Task<CacheLock<TKey>?> AcquireLockAsync<TKey>(TKey key, TimeSpan timeout, CancellationToken cancellationToken = default)
        where TKey : ICacheKey;

    Task<TEntity?> GetAsync<TEntity>(EntityCacheKey<TEntity> key, CancellationToken cancellationToken = default);

    Task RefreshAsync<TEntity>(EntityCacheKey<TEntity> key, CancellationToken cancellationToken = default);

    Task SetAsync<TEntity>(EntityCacheKey<TEntity> key, TEntity record, CancellationToken cancellationToken = default);

    Task SetWithLockAsync<TEntity>(EntityCacheKey<TEntity> key, CacheLock<EntityCacheKey<TEntity>> keyLock, TEntity record, CancellationToken cancellationToken = default);

    Task RemoveAsync<TEntity>(EntityCacheKey<TEntity> key, CancellationToken cancellationToken = default);
}
