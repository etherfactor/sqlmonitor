using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching;

/// <summary>
/// Provides means to cache and retrieve records and record sets, in Redis.
/// </summary>
internal class RedisRecordCache : IRecordCache
{
    private readonly ILogger _logger;
    private readonly IDatabase _database;
    private readonly IRedisHelperFactory _factory;

    public RedisRecordCache(
        ILogger<RedisRecordCache> logger,
        IDatabase database,
        IRedisHelperFactory factory)
    {
        _logger = logger;
        _database = database;
        _factory = factory;
    }

    /// <inheritdoc/>
    public ICacheEntity<TEntity> Entity<TEntity>(EntityCacheKey<TEntity> key)
        where TEntity : class, new()
    {
        return new RedisCacheEntity<TEntity>(_factory, _database, key);
    }

    /// <inheritdoc/>
    public ICacheEntitySet<TEntity> EntitySet<TEntity>()
        where TEntity : class, new()
    {
        return new RedisCacheEntitySet<TEntity>(_factory, _database);
    }
}
