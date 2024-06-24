using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching;

public class RedisIndexProperty<TEntity> : IRedisIndexProperty<TEntity>
    where TEntity : class, new()
{
    private readonly PropertyInfo _property;
    private readonly string _name;

    public string DisplayName => _name;
    public string PropertyName => _property.Name;
    public Type PropertyType => _property.PropertyType;

    public RedisIndexProperty(PropertyInfo property, ColumnAttribute attribute)
    {
        _property = property;
        _name = attribute.Name ?? property.Name;
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
