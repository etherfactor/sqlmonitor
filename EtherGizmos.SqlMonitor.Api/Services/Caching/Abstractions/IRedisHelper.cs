namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface IRedisHelper { }

public interface IRedisHelper<TEntity> : IRedisHelper
    where TEntity : class, new()
{
    IEnumerable<IRedisKeyProperty<TEntity>> GetKeyProperties();
    IEnumerable<IRedisLookupProperty<TEntity>> GetLookupSingleProperties();
}
