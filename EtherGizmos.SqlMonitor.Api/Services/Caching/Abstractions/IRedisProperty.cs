namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface IRedisProperty
{
    string DisplayName { get; }

    string PropertyName { get; }

    Type PropertyType { get; }
}

public interface IRedisProperty<TEntity> : IRedisProperty
    where TEntity : class, new()
{
    object? GetValue(TEntity entity);

    void SetValue(TEntity entity, object? value);
}
