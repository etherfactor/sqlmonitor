using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class MemoryLockedDistributedCache : ILockedDistributedCache
{
    //private readonly IMemoryCache _memoryCache;
    //private readonly IOptionsMonitor<CachingOptions> _optionsMonitor;

    //public MemoryLockedDistributedCache(
    //    IMemoryCache memoryCache,
    //    IOptionsMonitor<CachingOptions> optionsMonitor)
    //{
    //    _memoryCache = memoryCache;
    //    _optionsMonitor = optionsMonitor;
    //}

    //public Task<CacheLock<TKey>?> AcquireLockAsync<TKey>(TKey key, TimeSpan timeout, CancellationToken cancellationToken = default) where TKey : ICacheKey
    //{
    //    var @lock = new CacheLock<TKey>(key, new MockLock());
    //    return Task.FromResult<CacheLock<TKey>?>(@lock);
    //}

    //public Task<TEntity?> GetAsync<TEntity>(EntityCacheKey<TEntity> key, CancellationToken cancellationToken = default)
    //{
    //    if (_memoryCache.TryGet(key, out var record))
    //    {
    //        return Task.FromResult(record);
    //    }

    //    return Task.FromResult<TEntity?>(default);
    //}

    //public Task RefreshAsync<TEntity>(EntityCacheKey<TEntity> key, CancellationToken cancellationToken = default)
    //{
    //    return Task.CompletedTask;
    //}

    //public Task RemoveAsync<TEntity>(EntityCacheKey<TEntity> key, CancellationToken cancellationToken = default)
    //{
    //    _memoryCache.Remove(key);
    //    return Task.CompletedTask;
    //}

    //public Task SetAsync<TEntity>(EntityCacheKey<TEntity> key, TEntity record, CancellationToken cancellationToken = default)
    //{
    //    if (key.RequiresLock)
    //        throw new InvalidOperationException($"Key '{key.KeyName}' requires a lock to modify its value. First acquire a lock with '{nameof(AcquireLockAsync)}'.");

    //    var options = _optionsMonitor.CurrentValue.Keys.ContainsKey(key.Name)
    //        ? _optionsMonitor.CurrentValue.Keys[key.Name]
    //        : new DistributedCacheEntryOptions();

    //    options.AbsoluteExpirationRelativeToNow ??= TimeSpan.FromMinutes(5);
    //    options.AbsoluteExpirationRelativeToNow = (TimeSpan)options.AbsoluteExpirationRelativeToNow - TimeSpan.FromMilliseconds(500);

    //    _memoryCache.Set(key, record, DateTimeOffset.UtcNow + (TimeSpan)options.AbsoluteExpirationRelativeToNow);
    //    return Task.CompletedTask;
    //}

    //public Task SetWithLockAsync<TEntity>(EntityCacheKey<TEntity> key, CacheLock<EntityCacheKey<TEntity>> keyLock, TEntity record, CancellationToken cancellationToken = default)
    //{
    //    if (!keyLock.IsValid)
    //        throw new InvalidOperationException($"Key lock has expired. Acquire a new lock with '{nameof(AcquireLockAsync)}'.");

    //    var options = _optionsMonitor.CurrentValue.Keys.ContainsKey(key.Name)
    //        ? _optionsMonitor.CurrentValue.Keys[key.Name]
    //        : new DistributedCacheEntryOptions();

    //    options.AbsoluteExpirationRelativeToNow ??= TimeSpan.FromMinutes(5);
    //    options.AbsoluteExpirationRelativeToNow = (TimeSpan)options.AbsoluteExpirationRelativeToNow - TimeSpan.FromMilliseconds(500);

    //    _memoryCache.Set(key, record, DateTimeOffset.UtcNow + (TimeSpan)options.AbsoluteExpirationRelativeToNow);
    //    return Task.CompletedTask;
    //}

    //private class MockLock : IDistributedSynchronizationHandle
    //{
    //    public CancellationToken HandleLostToken => CancellationToken.None;

    //    public void Dispose()
    //    {
    //    }

    //    public ValueTask DisposeAsync()
    //    {
    //        return ValueTask.CompletedTask;
    //    }
    //}
    public Task<CacheLock<TKey>?> AcquireLockAsync<TKey>(TKey key, TimeSpan timeout, CancellationToken cancellationToken = default) where TKey : ICacheKey
    {
        throw new NotImplementedException();
    }

    public Task<TEntity?> GetAsync<TEntity>(EntityCacheKey<TEntity> key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RefreshAsync<TEntity>(EntityCacheKey<TEntity> key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync<TEntity>(EntityCacheKey<TEntity> key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync<TEntity>(EntityCacheKey<TEntity> key, TEntity record, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SetWithLockAsync<TEntity>(EntityCacheKey<TEntity> key, CacheLock<EntityCacheKey<TEntity>> keyLock, TEntity record, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
