using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using StackExchange.Redis;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Provides means to cache and retrieve records and record sets, in Redis.
/// </summary>
internal class RedisDistributedRecordCache : IDistributedRecordCache
{
    private readonly ILogger _logger;
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly IRedisHelperFactory _factory;

    public RedisDistributedRecordCache(
        ILogger<RedisDistributedRecordCache> logger,
        IConnectionMultiplexer multiplexer,
        IRedisHelperFactory factory)
    {
        _logger = logger;
        _multiplexer = multiplexer;
        _factory = factory;
    }

    /// <inheritdoc/>
    public ICacheEntity<TEntity> Entity<TEntity>(EntityCacheKey<TEntity> key)
        where TEntity : class, new()
    {
        return new RedisCacheEntity<TEntity>(_factory, _multiplexer.GetDatabase(), key);
    }

    /// <inheritdoc/>
    public ICacheEntitySet<TEntity> EntitySet<TEntity>()
        where TEntity : class, new()
    {
        return new RedisCacheEntitySet<TEntity>(_factory, _multiplexer.GetDatabase());
    }
}
