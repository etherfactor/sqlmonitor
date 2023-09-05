using Medallion.Threading;
using StackExchange.Redis;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class DistributedEntityCache
{
    private readonly ILogger _logger;
    private readonly IDatabase _database;
    private readonly IDistributedLockProvider _lockProvider;

    public DistributedEntityCache(
        ILogger<DistributedEntityCache> logger,
        IDatabase database,
        IDistributedLockProvider lockProvider)
    {
        _logger = logger;
        _database = database;
        _lockProvider = lockProvider;
    }

    /// <inheritdoc/>
    public async Task<CacheLock<TKey>?> AcquireLockAsync<TKey>(TKey key, TimeSpan timeout, CancellationToken cancellationToken = default)
        where TKey : ICacheKey
    {
        var lockName = $"{key.KeyName}:$lock";
        _logger.Log(LogLevel.Debug, "Attempting to acquire lock on {CacheKey}", lockName);

        var result = await _lockProvider.TryAcquireLockAsync(lockName, timeout, cancellationToken);
        if (result is not null)
        {
            var keyLock = new CacheLock<TKey>(key, result);
            return keyLock;
        }

        return null;
    }

    //public async Task SetAsync<TEntity>(EntityCacheKey<TEntity> key, TEntity entity)
    //{

    //}

    //public async Task AddAsync<TEntity>(EntitySetCacheKey<TEntity> key, TEntity entity)
    //{

    //}
}
