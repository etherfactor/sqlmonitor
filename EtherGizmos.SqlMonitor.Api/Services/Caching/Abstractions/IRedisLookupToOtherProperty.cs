namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface IRedisLookupToOtherProperty<TEntity> : IRedisProperty<TEntity>
    where TEntity : class, new()
{
    IRedisLookupFromOtherProperty? LookupProperty { get; }
    IDictionary<IRedisProperty<TEntity>, IRedisProperty> LookupAssociations { get; }
    bool LookupIsList { get; }
    bool LookupIsRecord { get; }

    TSubEntity GetLookupEntity<TSubEntity>(TEntity entity) where TSubEntity : class, new();
    TSubEntity GetLookupEntity<TSubEntity>(IDictionary<string, object?> entityProperties) where TSubEntity : class, new();
}
