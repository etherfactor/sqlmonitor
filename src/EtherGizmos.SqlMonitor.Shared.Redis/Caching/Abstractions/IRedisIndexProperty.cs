namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;

public interface IRedisIndexProperty<TEntity> : IRedisProperty<TEntity>
    where TEntity : class, new()
{
}
