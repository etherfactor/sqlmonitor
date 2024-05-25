namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;

public interface IRedisHelperFactory
{
    IRedisHelper<TEntity> CreateHelper<TEntity>()
        where TEntity : class, new();
}
