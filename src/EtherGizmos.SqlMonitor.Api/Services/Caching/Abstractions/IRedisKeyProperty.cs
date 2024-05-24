namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface IRedisKeyProperty
{
    int Position { get; }
}

public interface IRedisKeyProperty<TEntity> : IRedisKeyProperty, IRedisProperty<TEntity>
    where TEntity : class, new()
{
}
