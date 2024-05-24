using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class RedisProperty<TEntity> : IRedisProperty<TEntity>
    where TEntity : class, new()
{
    private readonly PropertyInfo _property;
    private readonly string _name;

    public string DisplayName => _name;
    public string PropertyName => _property.Name;
    public Type PropertyType => _property.PropertyType;

    public RedisProperty(PropertyInfo property, ColumnAttribute attribute)
    {
        _property = property;
        _name = attribute.Name ?? property.Name;
    }

    public void SetValue(TEntity entity, object? value)
    {
        if (_property.CanWrite)
            _property.SetValue(entity, value);
    }

    public object? GetValue(TEntity entity)
    {
        return _property.GetValue(entity);
    }
}
