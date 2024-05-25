namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;

public interface IRedisLookupFromOtherProperty : IRedisProperty
{
}

public interface IRedisLookupFromOtherProperty<TEntity> : IRedisLookupFromOtherProperty, IRedisProperty<TEntity>
    where TEntity : class, new()
{
}
