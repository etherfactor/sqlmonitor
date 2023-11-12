namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface IRedisIndexProperty<TEntity> : IRedisProperty<TEntity>
    where TEntity : class, new()
{
}
