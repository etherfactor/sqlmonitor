using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Models.Annotations;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class RedisLookupProperty<TEntity> : IRedisLookupProperty<TEntity>
    where TEntity : class, new()
{
    private readonly PropertyInfo _property;
    private readonly IEnumerable _test;

    public RedisLookupProperty(PropertyInfo property, LookupAttribute attribute, IEnumerable<IRedisProperty<TEntity>> properties)
    {
        _property = property;
        attribute.List;
    }

    public TSubEntity GetLookupEntity<TSubEntity>(TEntity entity)
    {

    }
}
