using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching;

public class RedisKeyProperty<TEntity> : IRedisKeyProperty<TEntity>
    where TEntity : class, new()
{
    private readonly PropertyInfo _property;
    private readonly string _name;
    private readonly int _position;

    public string DisplayName => _name;
    public string PropertyName => _property.Name;
    public Type PropertyType => _property.PropertyType;
    public int Position => _position;

    public RedisKeyProperty(PropertyInfo property, ColumnAttribute attribute, int position)
    {
        _property = property;
        _name = attribute.Name ?? property.Name;
        _position = position;
    }

    public void SetValue(TEntity entity, object? value)
    {
        _property.SetValue(entity, value);
    }

    public object? GetValue(TEntity entity)
    {
        return _property.GetValue(entity);
    }
}
