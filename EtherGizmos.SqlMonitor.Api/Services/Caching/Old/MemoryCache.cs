using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using System.Collections.Concurrent;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class MemoryCache : IMemoryCache
{
    private readonly ConcurrentDictionary<ICacheKey, CacheEntity> _cache;

    public MemoryCache()
    {
        _cache = new ConcurrentDictionary<ICacheKey, CacheEntity>();
    }

    public bool TryGet<TEntity>(EntityCacheKey<TEntity> key, out TEntity? record)
    {
        if (_cache.ContainsKey(key))
        {
            var data = _cache[key];
            if (data.Expiry > DateTimeOffset.UtcNow)
            {
                record = (TEntity?)data.Entity;
                return true;
            }
            else
            {
                _cache.TryRemove(key, out _);
            }
        }

        record = default;
        return false;
    }

    public void Remove<TEntity>(EntityCacheKey<TEntity> key)
    {
        _cache.TryRemove(key, out _);
    }

    public void Set<TEntity>(EntityCacheKey<TEntity> key, TEntity record, DateTimeOffset expiry)
    {
        var data = new CacheEntity(record, expiry);
        _cache.AddOrUpdate(key, data, (key, old) => data);
    }

    private class CacheEntity
    {
        public object? Entity { get; }

        public DateTimeOffset Expiry { get; }

        public CacheEntity(object? entity, DateTimeOffset expiry)
        {
            Entity = entity;
            Expiry = expiry;
        }
    }
}
