using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Models.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class RedisIndexProperty<TEntity> : IRedisIndexProperty<TEntity>
    where TEntity : class, new()
{
    private readonly PropertyInfo _property;
    private readonly string _name;
    private readonly bool _ignoreCase;

    public string DisplayName => _name;
    public string PropertyName => _property.Name;
    public Type PropertyType => _property.PropertyType;

    public RedisIndexProperty(PropertyInfo property, ColumnAttribute attribute)
    {
        _property = property;
        _name = attribute.Name ?? property.Name;
        _ignoreCase = property.GetCustomAttribute<IgnoreCaseAttribute>() is not null
            && _property.PropertyType.IsAssignableTo(typeof(string));
    }

    public object? GetValue(TEntity entity)
    {
        var value = _property.GetValue(entity);
        if (_ignoreCase)
        {
            value = (value as string)!.ToUpper();
        }

        return value;
    }

    public void SetValue(TEntity entity, object? value)
    {
        _property.SetValue(entity, value);
    }
}
