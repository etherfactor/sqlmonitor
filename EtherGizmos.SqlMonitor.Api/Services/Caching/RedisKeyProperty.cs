using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class RedisKeyProperty<TEntity> : IRedisKeyProperty<TEntity>
    where TEntity : class, new()
{
    private readonly PropertyInfo _property;
    private readonly string _name;
    private readonly int _position;

    public string Name => _name;
    public Type Type => _property.PropertyType;
    public int Position => _position;

    public RedisKeyProperty(PropertyInfo property, ColumnAttribute attribute, int position)
    {
        _property = property;
        _name = attribute.Name ?? property.Name;
        _position = position;
    }

    public void SetValue(TEntity entity, object value)
    {
        _property.SetValue(entity, value);
    }

    public object? GetValue(TEntity entity)
    {
        return _property.GetValue(entity);
    }
}
