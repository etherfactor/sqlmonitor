using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Models.Annotations;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class RedisLookupToOtherProperty<TEntity> : IRedisLookupToOtherProperty<TEntity>
    where TEntity : class, new()
{
    private readonly IRedisHelperFactory _factory;

    private readonly PropertyInfo _property;
    private readonly bool _isList;
    private readonly bool _isRecord;
    private IDictionary<IRedisProperty<TEntity>, IRedisProperty> _associations = new Dictionary<IRedisProperty<TEntity>, IRedisProperty>();
    private IRedisLookupFromOtherProperty? _lookup;

    public string DisplayName => "(unnamed)";
    public string PropertyName => _property.Name;
    public Type PropertyType => _property.PropertyType;
    public bool LookupIsList => _isList;
    public bool LookupIsRecord => _isRecord;
    public IDictionary<IRedisProperty<TEntity>, IRedisProperty> LookupAssociations => _associations;
    public IRedisLookupFromOtherProperty? LookupProperty => _lookup;

    public RedisLookupToOtherProperty(PropertyInfo property, LookupAttribute attribute, IRedisHelperFactory factory)
    {
        _factory = factory;

        _property = property;

        if (attribute.List is not null && attribute.Record is not null)
            throw new InvalidOperationException("");

        _isList = attribute.List is not null;
        _isRecord = attribute.Record is not null;

        //Open a generic context to interact with the lookup property type
        var method = typeof(RedisLookupToOtherProperty<TEntity>)
            .GetMethod(nameof(AssociateProperties), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(PropertyType);
        method.Invoke(this, new object?[] { attribute.IdProperties, attribute.List ?? attribute.Record });
    }

    private void AssociateProperties<TSubEntity>(IEnumerable<(string ForeignKey, string PrimaryKey)> idProperties, string? lookupName)
        where TSubEntity : class, new()
    {
        var helper = _factory.CreateHelper<TEntity>();
        var properties = helper.GetProperties();

        var subHelper = _factory.CreateHelper<TSubEntity>();
        var subProperties = subHelper.GetProperties();

        _lookup = subHelper.GetLookupSetProperties()
            .SingleOrDefault(e => e.PropertyName == lookupName);

        foreach (var idProperty in idProperties)
        {
            var property = properties.Single(e => e.PropertyName == idProperty.ForeignKey);
            var subProperty = subProperties.Single(e => e.PropertyName == idProperty.PrimaryKey);

            _associations.Add(property, subProperty);
        }
    }

    public TSubEntity GetLookupEntity<TSubEntity>(IDictionary<string, object?> entityProperties)
        where TSubEntity : class, new()
    {
        if (typeof(TSubEntity) != PropertyType)
            throw new InvalidOperationException($"The specified generic type must match the lookup property type ({PropertyType}).");

        var subEntity = new TSubEntity();

        foreach (var association in _associations)
        {
            var property = association.Key;
            var subProperty = (IRedisProperty<TSubEntity>)association.Value;

            var value = entityProperties[property.PropertyName];
            subProperty.SetValue(subEntity, value);
        }

        return subEntity;
    }

    public TSubEntity GetLookupEntity<TSubEntity>(TEntity entity)
        where TSubEntity : class, new()
    {
        if (typeof(TSubEntity) != PropertyType)
            throw new InvalidOperationException($"The specified generic type must match the lookup property type ({PropertyType}).");

        var subEntity = new TSubEntity();

        foreach (var association in _associations)
        {
            var property = association.Key;
            var subProperty = (IRedisProperty<TSubEntity>)association.Value;

            var value = property.GetValue(entity);
            subProperty.SetValue(subEntity, value);
        }

        return subEntity;
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
