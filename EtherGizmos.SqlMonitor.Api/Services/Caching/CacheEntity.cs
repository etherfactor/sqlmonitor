using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using StackExchange.Redis;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class CacheEntity<TEntity> : ICacheEntity<TEntity>
    where TEntity : new()
{
    private readonly IDatabase _database;
    private readonly EntityCacheKey<TEntity> _key;

    public CacheEntity(IDatabase database, EntityCacheKey<TEntity> key)
    {
        _database = database;
        _key = key;
    }

    public async Task DeleteAsync()
    {
        var serializer = RedisHelperCache.For<TEntity>();
        var action = serializer.GetDeleteAction(_key);
        await action(_database);
    }

    public async Task SetAsync(TEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var serializer = RedisHelperCache.For<TEntity>();
        var action = serializer.GetSetAction(_key, entity);
        await action(_database);
    }
}
