namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface IRedisKeyProperty<TEntity> : IRedisProperty<TEntity>
    where TEntity : class, new()
{
    int Position { get; }
}
