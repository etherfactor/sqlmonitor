using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using StackExchange.Redis;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Provides means to cache and retrieve records and record sets in Redis.
/// </summary>
public class RedisDistributedRecordCache : IDistributedRecordCache
{
    private readonly IConnectionMultiplexer _multiplexer;

    public RedisDistributedRecordCache(IConnectionMultiplexer multiplexer)
    {
        _multiplexer = multiplexer;
    }

    /// <inheritdoc/>
    public ICacheEntity<TEntity> Entity<TEntity>(EntityCacheKey<TEntity> key)
        where TEntity : new()
    {
        return new CacheEntity<TEntity>(_multiplexer.GetDatabase(), key);
    }

    /// <inheritdoc/>
    public ICacheEntitySet<TEntity> EntitySet<TEntity>()
        where TEntity : new()
    {
        return new CacheEntitySet<TEntity>(_multiplexer.GetDatabase());
    }
}
