namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface ILockedDistributedCache
{
    Task<CacheLock<TEntity>?> AcquireLockAsync<TEntity>(CacheKey<TEntity> key, TimeSpan timeout, CancellationToken cancellationToken = default);

    Task<TEntity?> GetAsync<TEntity>(CacheKey<TEntity> key, CancellationToken cancellationToken = default);

    Task RefreshAsync<TEntity>(CacheKey<TEntity> key, CancellationToken cancellationToken = default);

    Task SetAsync<TEntity>(CacheKey<TEntity> key, TEntity record, CancellationToken cancellationToken = default);

    Task SetWithLockAsync<TEntity>(CacheKey<TEntity> key, CacheLock<TEntity> keyLock, TEntity record, CancellationToken cancellationToken = default);

    Task RemoveAsync<TEntity>(CacheKey<TEntity> key, CancellationToken cancellationToken = default);
}
