namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface IRedisHelperFactory
{
    IRedisHelper<TEntity> CreateHelper<TEntity>()
        where TEntity : class, new();
}
