using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using Medallion.Threading;
using StackExchange.Redis;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Provides means to cache and retrieve records and record sets, in Redis.
/// </summary>
internal class RedisDistributedRecordCache : IDistributedRecordCache
{
    private readonly ILogger _logger;
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly IDistributedLockProvider _distributedLockProvider;

    public RedisDistributedRecordCache(
        ILogger<RedisDistributedRecordCache> logger,
        IConnectionMultiplexer multiplexer,
        IDistributedLockProvider distributedLockProvider)
    {
        _logger = logger;
        _multiplexer = multiplexer;
        _distributedLockProvider = distributedLockProvider;
    }

    /// <inheritdoc/>
    public async Task<CacheLock<TKey>?> AcquireLockAsync<TKey>(TKey key, TimeSpan timeout, CancellationToken cancellationToken = default)
        where TKey : ICacheKey
    {
        var lockName = $"{Constants.CacheSchemaName}:{key.KeyName}:{Constants.CacheLockSuffix}";
        _logger.Log(LogLevel.Debug, "Attempting to acquire lock on {CacheKey}", lockName);

        var result = await _distributedLockProvider.TryAcquireLockAsync(lockName, timeout, cancellationToken);
        if (result is not null)
        {
            var keyLock = new CacheLock<TKey>(key, result);
            return keyLock;
        }

        return null;
    }

    /// <inheritdoc/>
    public ICacheEntity<TEntity> Entity<TEntity>(EntityCacheKey<TEntity> key)
        where TEntity : new()
    {
        return new RedisCacheEntity<TEntity>(_multiplexer.GetDatabase(), key);
    }

    /// <inheritdoc/>
    public ICacheEntitySet<TEntity> EntitySet<TEntity>()
        where TEntity : new()
    {
        return new RedisCacheEntitySet<TEntity>(_multiplexer.GetDatabase());
    }
}
