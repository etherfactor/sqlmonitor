namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public interface IMemoryCache
{
    bool TryGet<TEntity>(EntityCacheKey<TEntity> key, out TEntity? record);

    void Set<TEntity>(EntityCacheKey<TEntity> key, TEntity record, DateTimeOffset expiry);

    void Remove<TEntity>(EntityCacheKey<TEntity> key);
}
