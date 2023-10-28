namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface IRedisProperty<TEntity>
    where TEntity : class, new()
{
    string Name { get; }

    Type Type { get; }

    object? GetValue(TEntity entity);

    void SetValue(TEntity entity, object? value);
}
