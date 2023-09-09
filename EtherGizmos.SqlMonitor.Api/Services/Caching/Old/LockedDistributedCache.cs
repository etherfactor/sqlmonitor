using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Functions similarly to an <see cref="IDistributedCache"/>, but with locking functionality.
/// </summary>
public class LockedDistributedCache : ILockedDistributedCache
{
    //private readonly ILogger _logger;
    //private readonly IDistributedCache _distributedCache;
    //private readonly IDistributedLockProvider _distributedLockProvider;
    //private readonly IOptionsMonitor<CachingOptions> _optionsMonitor;

    //public LockedDistributedCache(
    //    ILogger<LockedDistributedCache> logger,
    //    IDistributedCache distributedCache,
    //    IDistributedLockProvider distributedLockProvider,
    //    IOptionsMonitor<CachingOptions> optionsMonitor)
    //{
    //    _logger = logger;
    //    _distributedCache = distributedCache;
    //    _distributedLockProvider = distributedLockProvider;
    //    _optionsMonitor = optionsMonitor;
    //}

    ///// <inheritdoc/>
    //public async Task<CacheLock<TKey>?> AcquireLockAsync<TKey>(TKey key, TimeSpan timeout, CancellationToken cancellationToken = default)
    //    where TKey : ICacheKey
    //{
    //    var lockName = $"{key.KeyName}:$lock";
    //    _logger.Log(LogLevel.Debug, "Attempting to acquire lock on {CacheKey}", lockName);

    //    var result = await _distributedLockProvider.TryAcquireLockAsync(lockName, timeout, cancellationToken);
    //    if (result is not null)
    //    {
    //        var keyLock = new CacheLock<TKey>(key, result);
    //        return keyLock;
    //    }

    //    return null;
    //}

    ///// <inheritdoc/>
    //public async Task<TEntity?> GetAsync<TEntity>(EntityCacheKey<TEntity> key, CancellationToken cancellationToken = default)
    //{
    //    var lockName = key.KeyName;
    //    _logger.Log(LogLevel.Information, "Retrieving cached entity at {CacheKey}", lockName);

    //    var result = await _distributedCache.GetAsync(lockName, cancellationToken);
    //    if (result is not null)
    //    {
    //        var stream = new MemoryStream(result);
    //        var data = await JsonSerializer.DeserializeAsync<TEntity>(stream);

    //        return data;
    //    }

    //    return default;
    //}

    ///// <inheritdoc/>
    //public async Task RefreshAsync<TEntity>(EntityCacheKey<TEntity> key, CancellationToken cancellationToken = default)
    //{
    //    var lockName = key.KeyName;
    //    _logger.Log(LogLevel.Information, "Refreshing cached entity at {CacheKey}", lockName);

    //    await _distributedCache.RefreshAsync(lockName, cancellationToken); ;
    //}

    ///// <inheritdoc/>
    //public async Task RemoveAsync<TEntity>(EntityCacheKey<TEntity> key, CancellationToken cancellationToken = default)
    //{
    //    var lockName = key.KeyName;
    //    _logger.Log(LogLevel.Information, "Removing cached entity at {CacheKey}", lockName);

    //    await _distributedCache.RemoveAsync(lockName, cancellationToken);
    //}

    ///// <inheritdoc/>
    //public async Task SetAsync<TEntity>(EntityCacheKey<TEntity> key, TEntity record, CancellationToken cancellationToken = default)
    //{
    //    if (key.RequiresLock)
    //        throw new InvalidOperationException($"Key '{key.KeyName}' requires a lock to modify its value. First acquire a lock with '{nameof(AcquireLockAsync)}'.");

    //    var lockName = key.KeyName;
    //    _logger.Log(LogLevel.Information, "Setting cached entity at {CacheKey}", lockName);

    //    var options = _optionsMonitor.CurrentValue.Keys.ContainsKey(key.Name)
    //        ? _optionsMonitor.CurrentValue.Keys[key.Name]
    //        : new DistributedCacheEntryOptions();

    //    options.AbsoluteExpirationRelativeToNow ??= TimeSpan.FromMinutes(5);
    //    options.AbsoluteExpirationRelativeToNow = (TimeSpan)options.AbsoluteExpirationRelativeToNow - TimeSpan.FromMilliseconds(500);

    //    var data = JsonSerializer.Serialize(record);
    //    var bytes = Encoding.UTF8.GetBytes(data);
    //    await _distributedCache.SetAsync(lockName, bytes, options, cancellationToken);
    //}

    ///// <inheritdoc/>
    //public async Task SetWithLockAsync<TEntity>(EntityCacheKey<TEntity> key, CacheLock<EntityCacheKey<TEntity>> keyLock, TEntity record, CancellationToken cancellationToken = default)
    //{
    //    if (!keyLock.IsValid)
    //        throw new InvalidOperationException($"Key lock has expired. Acquire a new lock with '{nameof(AcquireLockAsync)}'.");

    //    var lockName = key.KeyName;
    //    _logger.Log(LogLevel.Information, "Setting cached entity with lock at {CacheKey}", lockName);

    //    var options = _optionsMonitor.CurrentValue.Keys.ContainsKey(key.Name)
    //        ? _optionsMonitor.CurrentValue.Keys[key.Name]
    //        : new DistributedCacheEntryOptions();

    //    options.AbsoluteExpirationRelativeToNow ??= TimeSpan.FromMinutes(5);
    //    options.AbsoluteExpirationRelativeToNow = (TimeSpan)options.AbsoluteExpirationRelativeToNow - TimeSpan.FromMilliseconds(500);

    //    var data = JsonSerializer.Serialize(record);
    //    var bytes = Encoding.UTF8.GetBytes(data);
    //    await _distributedCache.SetAsync(lockName, bytes, options, cancellationToken);
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
