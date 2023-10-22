using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using StackExchange.Redis;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Provides means for caching and retrieving a single entity, in Redis.
/// </summary>
/// <typeparam name="TEntity">The type of entity being cached.</typeparam>
internal class RedisCacheEntity<TEntity> : ICacheEntity<TEntity>
    where TEntity : class, new()
{
    private readonly IDatabase _database;
    private readonly EntityCacheKey<TEntity> _key;

    public RedisCacheEntity(IDatabase database, EntityCacheKey<TEntity> key)
    {
        _database = database;
        _key = key;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        var serializer = RedisHelperCache.For<TEntity>();
        var action = serializer.GetDeleteAction(_key);
        await action(_database);
    }

    /// <inheritdoc/>
    public async Task<TEntity?> GetAsync(CancellationToken cancellationToken = default)
    {
        var serializer = RedisHelperCache.For<TEntity>();
        var builder = serializer.GetReadActionBuilder(_key);

        var action = builder(_database);
        return await action();
    }

    /// <inheritdoc/>
    public async Task SetAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var serializer = RedisHelperCache.For<TEntity>();
        var builder = serializer.GetSetActionBuilder(_key, entity);

        var action = builder(_database);
        await action();
    }
}
