using Medallion.Threading;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Storage;

public class LockedDistributedCache : ILockedDistributedCache
{
    private IDistributedCache _distributedCache;
    private IDistributedLockProvider _distributedLockProvider;

    public LockedDistributedCache(
        IDistributedCache distributedCache,
        IDistributedLockProvider distributedLockProvider)
    {
        _distributedCache = distributedCache;
        _distributedLockProvider = distributedLockProvider;
    }

    public async Task<bool> TryAcquireLockAsync<TEntity>(CacheKey<TEntity> key, TimeSpan timeout, Action<CacheLock<TEntity>> @out, CancellationToken cancellationToken = default)
    {
        var result = await _distributedLockProvider.TryAcquireLockAsync($"lock:{key.Name}", timeout, cancellationToken);
        if (result is not null)
        {
            var keyLock = new CacheLock<TEntity>(key, result);

            @out(keyLock);

            return true;
        }

        return false;
    }

    public async Task<bool> TryGetAsync<TEntity>(CacheKey<TEntity> key, Action<TEntity?> @out, CancellationToken cancellationToken = default)
    {
        var result = await _distributedCache.GetAsync(key.Name, cancellationToken);
        if (result is not null)
        {
            var stream = new MemoryStream(result);
            var data = await JsonSerializer.DeserializeAsync<TEntity>(stream);

            @out(data);

            return true;
        }

        return false;
    }

    public async Task<bool> TryRefreshAsync<TEntity>(CacheKey<TEntity> key, TEntity record, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RefreshAsync(key.Name, cancellationToken);
        return true;
    }

    public async Task<bool> TryRemoveAsync<TEntity>(CacheKey<TEntity> key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key.Name, cancellationToken);
        return true;
    }

    public async Task<bool> TrySetAsync<TEntity>(CacheKey<TEntity> key, TEntity record, DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (key.RequiresLock)
            throw new InvalidOperationException($"Key '{key.Name}' requires a lock to modify its value. First acquire a lock with '{nameof(TryAcquireLockAsync)}'.");

        options ??= new DistributedCacheEntryOptions();

        var data = JsonSerializer.Serialize(record);
        var bytes = Encoding.UTF8.GetBytes(data);
        await _distributedCache.SetAsync(key.Name, bytes, options, cancellationToken);
        return true;
    }

    public async Task<bool> TrySetWithLockAsync<TEntity>(CacheKey<TEntity> key, CacheLock<TEntity> keyLock, TEntity record, DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (!keyLock.IsValid)
            throw new InvalidOperationException($"Key lock has expired. Acquire a new lock with '{nameof(TryAcquireLockAsync)}'.");

        options ??= new DistributedCacheEntryOptions();

        var data = JsonSerializer.Serialize(record);
        var bytes = Encoding.UTF8.GetBytes(data);
        await _distributedCache.SetAsync(key.Name, bytes, options, cancellationToken);
        return true;
    }
}
