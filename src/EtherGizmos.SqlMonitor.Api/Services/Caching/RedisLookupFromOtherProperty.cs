using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Models.Annotations;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class RedisLookupFromOtherProperty<TEntity> : IRedisLookupFromOtherProperty<TEntity>
    where TEntity : class, new()
{
    private readonly IRedisHelperFactory _factory;

    private readonly PropertyInfo _property;
    private readonly string _name;

    public string DisplayName => _name;
    public string PropertyName => _property.Name;
    public Type PropertyType => _property.PropertyType;

    public RedisLookupFromOtherProperty(PropertyInfo property, LookupIndexAttribute attribute, IRedisHelperFactory factory)
    {
        _factory = factory;

        _property = property;
        _name = attribute.Name;
    }

    public object? GetValue(TEntity entity)
    {
        return _property.GetValue(entity);
    }

    public void SetValue(TEntity entity, object? value)
    {
        _property.SetValue(entity, value);
    }
}
