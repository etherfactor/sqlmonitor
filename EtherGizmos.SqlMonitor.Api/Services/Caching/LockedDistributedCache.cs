using EtherGizmos.SqlMonitor.Api.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using Medallion.Threading;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class LockedDistributedCache : ILockedDistributedCache
{
    private IDistributedCache _distributedCache;
    private IDistributedLockProvider _distributedLockProvider;
    private IOptionsMonitor<CachingOptions> _optionsMonitor;

    public LockedDistributedCache(
        IDistributedCache distributedCache,
        IDistributedLockProvider distributedLockProvider,
        IOptionsMonitor<CachingOptions> optionsMonitor)
    {
        _distributedCache = distributedCache;
        _distributedLockProvider = distributedLockProvider;
        _optionsMonitor = optionsMonitor;
    }

    public async Task<CacheLock<TEntity>?> AcquireLockAsync<TEntity>(CacheKey<TEntity> key, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var result = await _distributedLockProvider.TryAcquireLockAsync($"lock:{key.Name}", timeout, cancellationToken);
        if (result is not null)
        {
            var keyLock = new CacheLock<TEntity>(key, result);
            return keyLock;
        }

        return null;
    }

    public async Task<TEntity?> GetAsync<TEntity>(CacheKey<TEntity> key, CancellationToken cancellationToken = default)
    {
        var result = await _distributedCache.GetAsync(key.Name, cancellationToken);
        if (result is not null)
        {
            var stream = new MemoryStream(result);
            var data = await JsonSerializer.DeserializeAsync<TEntity>(stream);

            return data;
        }

        return default;
    }

    public async Task RefreshAsync<TEntity>(CacheKey<TEntity> key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RefreshAsync(key.Name, cancellationToken); ;
    }

    public async Task RemoveAsync<TEntity>(CacheKey<TEntity> key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key.Name, cancellationToken);
    }

    public async Task SetAsync<TEntity>(CacheKey<TEntity> key, TEntity record, CancellationToken cancellationToken = default)
    {
        if (key.RequiresLock)
            throw new InvalidOperationException($"Key '{key.Name}' requires a lock to modify its value. First acquire a lock with '{nameof(AcquireLockAsync)}'.");

        var options = _optionsMonitor.CurrentValue.Keys.ContainsKey(key.Name)
            ? _optionsMonitor.CurrentValue.Keys[key.Name]
            : new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

        var data = JsonSerializer.Serialize(record);
        var bytes = Encoding.UTF8.GetBytes(data);
        await _distributedCache.SetAsync(key.Name, bytes, options, cancellationToken);
    }

    public async Task SetWithLockAsync<TEntity>(CacheKey<TEntity> key, CacheLock<TEntity> keyLock, TEntity record, CancellationToken cancellationToken = default)
    {
        if (!keyLock.IsValid)
            throw new InvalidOperationException($"Key lock has expired. Acquire a new lock with '{nameof(AcquireLockAsync)}'.");

        var options = _optionsMonitor.CurrentValue.Keys.ContainsKey(key.Name)
            ? _optionsMonitor.CurrentValue.Keys[key.Name]
            : new DistributedCacheEntryOptions();

        options.AbsoluteExpirationRelativeToNow ??= TimeSpan.FromMinutes(5);
        options.AbsoluteExpirationRelativeToNow = (TimeSpan)options.AbsoluteExpirationRelativeToNow - TimeSpan.FromMilliseconds(500);

        var data = JsonSerializer.Serialize(record);
        var bytes = Encoding.UTF8.GetBytes(data);
        await _distributedCache.SetAsync(key.Name, bytes, options, cancellationToken);
    }
}
