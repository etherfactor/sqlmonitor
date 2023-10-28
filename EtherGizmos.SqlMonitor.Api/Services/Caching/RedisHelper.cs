using Castle.DynamicProxy;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Models.Annotations;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.Json;
using System.Web;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

internal static class RedisHelperProxy
{
    internal static readonly ProxyGenerator Generator = new ProxyGenerator();
}

/// <summary>
/// Provides helper methods for working with Redis caching.
/// </summary>
/// <typeparam name="TEntity">The type of entity being cached.</typeparam>
public class RedisHelper<TEntity> : IRedisHelper<TEntity>
    where TEntity : class, new()
{
    private const char IdSeparator = '|';

    private readonly IRedisHelperFactory _factory;

    private readonly string _tableKey;
    private readonly IEnumerable<IRedisKeyProperty<TEntity>> _keys;
    private readonly IEnumerable<IRedisProperty<TEntity>> _indexes;
    private readonly IEnumerable<IRedisLookupProperty<TEntity>> _lookupSingles;
    private readonly IEnumerable<(PropertyInfo Property, LookupIndexAttribute LookupIndex)> _lookupSets;
    private readonly IEnumerable<IRedisProperty<TEntity>> _properties;

    /// <summary>
    /// Use <see cref="RedisHelperFactory.For{TEntity}"/> instead!
    /// </summary>
    public RedisHelper(IRedisHelperFactory factory)
    {
        _factory = factory;

        //Ensure the entity has a TableAttribute
        var tableAttribute = typeof(TEntity).GetCustomAttribute<TableAttribute>()
            ?? throw new InvalidOperationException($"To cache {typeof(TEntity).Name}, it must be annotated with a {nameof(TableAttribute)}.");
        _tableKey = tableAttribute.Name;

        //Can only serialize public, non-static properties
        var allProperties = typeof(TEntity)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public);

        var clrProperties = allProperties
            .Where(x => x.PropertyType.IsSerializable);

        //Limit properties to those with ColumnAttribute
        var sortedProperties = clrProperties
            .Select(e =>
            {
                var attribute = e.GetCustomAttribute<ColumnAttribute>()!;
                return (Property:e, Column:attribute);
            })
            .Where(e => e.Column is not null)
            .OrderBy(e => e.Column.Name).ToList();

        //Keys define a KeyAttribute
        _keys = sortedProperties
            .Where(e => e.Property.GetCustomAttribute<KeyAttribute>() is not null)
            .Select(e => new RedisKeyProperty<TEntity>(e.Property, e.Column))
            .ToList();
        ValidateKeys();

        //Properties are any non-object properties
        _properties = sortedProperties
            //.Where(e => e.Property.GetCustomAttribute<KeyAttribute>() is null)
            .Where(e => !e.Property.PropertyType.IsComplexType())
            .Select(e => new RedisProperty<TEntity>(e.Property, e.Column))
            .ToList();

        //Indexes define an IndexedAttribute
        _indexes = sortedProperties
            .Where(e => e.Property.GetCustomAttribute<IndexedAttribute>() is not null)
            .Select(e => new RedisProperty<TEntity>(e.Property, e.Column))
            .ToList();
        ValidateIndexes();

        //Lookups define a LookupAttribute if the current object looks up to another
        _lookupSingles = allProperties
            .Select(e =>
            {
                var attribute = e.GetCustomAttribute<LookupAttribute>()!;
                return (Property: e, Attribute: attribute);
            })
            .Where(e => e.Attribute is not null)
            .Select(e => new RedisLookupProperty<TEntity>(e.Property, e.Attribute, _properties))
            .ToList();
        ValidateLookupSingles();

        //Lookups define a LookupIndexAttribute if another object looks up to this one
        _lookupSets = allProperties
            .Select(e =>
            {
                var Property = e;
                var Attribute = e.GetCustomAttribute<LookupIndexAttribute>()!;
                return (Property, Attribute);
            })
            .Where(e => e.Attribute is not null)
            .ToList();
        ValidateLookupSets();
    }

    #region Validation
    private void ValidateKeys()
    {
        if (!_keys.Any())
            throw new InvalidOperationException($"To cache {typeof(TEntity).Name}, at least one of its properties must be annotated with a {nameof(KeyAttribute)}.");
    }

    private void ValidateIndexes()
    {
    }

    private void ValidateLookupSingles()
    {
        //TODO: Make sure that the lookup singles reference keys correctly
    }

    private void ValidateLookupSets()
    {
    }
    #endregion Validation

    #region Keys & Values
    public IEnumerable<IRedisKeyProperty<TEntity>> GetKeyProperties() => _keys;

    public IEnumerable<IRedisLookupProperty<TEntity>> GetLookupSingleProperties() => _lookupSingles;

    /// <summary>
    /// Constructs the Redis key for a given entity.
    /// </summary>
    /// <param name="key">The entity for which to calculate the key.</param>
    /// <returns>The Redis key for the entity.</returns>
    public RedisKey GetEntityKey(EntityCacheKey<TEntity> key)
    {
        var keyData = new RedisKey($"{Constants.Cache.SchemaName}:$$entity:{key.KeyName}");
        return keyData;
    }

    /// <summary>
    /// Constructs the entity's id.
    /// </summary>
    /// <param name="entity">The entity for which to construct the id.</param>
    /// <returns>The entity's id.</returns>
    private RedisValue GetRecordId(TEntity entity)
    {
        return GetRecordId(_keys.Select(e => e.GetValue(entity)!));
    }

    internal RedisValue GetRecordId(IEnumerable<object> keys)
    {
        var raw = keys.Select(e => JsonSerializer.Serialize(e))
            .Select(e => Encode(e));
        var value = new RedisValue(string.Join(IdSeparator, raw));

        return value;
    }

    /// <summary>
    /// Constructs the Redis key for a given entity in a set.
    /// </summary>
    /// <param name="entity">The entity for which to calculate the key.</param>
    /// <returns>The Redis key for the entity.</returns>
    public RedisKey GetSetEntityKey(TEntity entity)
    {
        var value = new RedisKey($"{Constants.Cache.SchemaName}:$$table:{_tableKey}:{GetRecordId(entity)}");
        return value;
    }

    /// <summary>
    /// Constructs the set's primary key.
    /// </summary>
    /// <returns>The set's primary key.</returns>
    private RedisKey GetEntitySetPrimaryKey()
    {
        var keyData = new RedisKey($"{Constants.Cache.SchemaName}:$$table:{_tableKey}:$$primary");
        return keyData;
    }

    /// <summary>
    /// Constructs the set's index for the specified property.
    /// </summary>
    /// <param name="index">The indexed property.</param>
    /// <returns>The set's index key.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private RedisKey GetEntitySetIndexKey(PropertyInfo index)
    {
        var indexAttribute = index.GetCustomAttribute<IndexedAttribute>()
            ?? throw new InvalidOperationException($"Can only get an index for an indexed property, specifying an {nameof(IndexedAttribute)}.");

        var keyData = new RedisKey($"{Constants.Cache.SchemaName}:$$table:{_tableKey}:${index.GetCustomAttribute<ColumnAttribute>()!.Name!.ToSnakeCase()}");
        return keyData;
    }

    /// <summary>
    /// Constructs the set's lookup key for the specified property.
    /// </summary>
    /// <param name="lookup">The lookup property.</param>
    /// <returns>The set's lookup key.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private RedisKey GetEntitySetLookupSetKey(PropertyInfo lookup)
    {
        var lookupAttribute = lookup.GetCustomAttribute<LookupIndexAttribute>()
            ?? throw new InvalidOperationException($"Can only get a lookup for a lookup property, specifying a {nameof(LookupIndexAttribute)}.");

        var lookupData = new RedisKey($"{Constants.Cache.SchemaName}:$$table:{_tableKey}:${lookup.GetCustomAttribute<ColumnAttribute>()!.Name!.ToSnakeCase()}");
        return lookupData;
    }

    private (RedisKey, RedisValue)? GetEntitySetLookup(PropertyInfo lookup, TEntity entity)
    {
        var lookupAttribute = _lookupSingles.Single(e => e.Item1 == lookup).Item2;

        if (lookupAttribute.List is not null && lookupAttribute.Record is not null)
            throw new InvalidOperationException($"{lookup.DeclaringType?.Name}.{lookup.Name} specifies both a list and record lookup.");

        if (lookupAttribute.List is not null)
        {
            var lookupProperty = lookup.PropertyType
                .GetProperty(lookupAttribute.List, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new InvalidOperationException($"{typeof(TEntity).Name} contains a {nameof(LookupAttribute)} " +
                $"pointing at {lookupAttribute.List}, which does not exist.");

            var lookupIndexAttribute = lookupProperty
                .GetCustomAttribute<LookupIndexAttribute>()
                ?? throw new InvalidOperationException($"{typeof(TEntity).Name} contains a {nameof(LookupAttribute)}, " +
                $"but {lookup.PropertyType.Name} does not specify a {nameof(LookupIndexAttribute)}.");

            var lookupProperties = lookupAttribute.IdProperties
                .Select(e => _properties.Single(p => p.Name == e.ForeignKey))
                .OrderBy(e => e.Item2.Name);

            var maybeLookupValues = lookupProperties
                .Select(e => e.Item1.GetValue(entity));

            if (maybeLookupValues.All(e => e is not null))
            {
                var lookupValues = maybeLookupValues.Select(e => JsonSerializer.Serialize(e))
                    .Select(e => Encode(e));

                var lookupTableKey = lookup.PropertyType.GetCustomAttribute<TableAttribute>()!.Name;

                var redisKeyValue = new RedisValue(string.Join(IdSeparator, lookupValues));
                var redisKey = new RedisKey($"{Constants.Cache.SchemaName}:$$table:{_tableKey}:{lookupTableKey}:{redisKeyValue}:${lookupIndexAttribute.Name.ToSnakeCase()}");

                var redisValue = GetRecordId(entity);

                return (redisKey, redisValue);
            }
        }
        else if (lookupAttribute.Record is not null)
        {
            throw new NotImplementedException();
        }

        return null;
    }

    /// <summary>
    /// Generate a temporary key.
    /// </summary>
    /// <returns>The temporary key.</returns>
    internal RedisKey GetTempKey()
    {
        var guid = Guid.NewGuid();
        var tempKey = new RedisKey($"{Constants.Cache.SchemaName}:$$temp:{guid}");
        return tempKey;
    }

    /// <summary>
    /// Constructs the Redis properties for reading the entity.
    /// </summary>
    /// <returns>The Redis properties.</returns>
    private RedisValue[] GetReadProperties()
    {
        var propertyNames = _properties.Select(e => e.Name.ToSnakeCase())
            .Select(e => new RedisValue(e))
            .ToArray();

        return propertyNames;
    }

    /// <summary>
    /// Constructs the Redis properties for listing the entity.
    /// </summary>
    /// <returns>The Redis properties.</returns>
    private RedisValue[] GetListProperties()
    {
        var propertyNames = _properties
            .Select(e => $"{Constants.Cache.SchemaName}:$$table:{_tableKey}:*->{e.Name.ToSnakeCase()}")
            .Select(e => new RedisValue(e))
            .ToArray();

        return propertyNames;
    }

    /// <summary>
    /// Constructs the Redis keys for entity lookups.
    /// </summary>
    /// <returns>The Redis keys.</returns>
    private RedisKey[] GetLookupKeys(TEntity entity)
    {
        var lookupNames = _lookupSets
            .Select(e => $"{Constants.Cache.SchemaName}:$$table:{_tableKey}:{GetRecordId(entity)}:{e.Item1.Name}")
            .Select(e => new RedisKey(e))
            .ToArray();

        return lookupNames;
    }
    #endregion Keys & Values

    #region Encoding
    /// <summary>
    /// Encodes a value, for writing to Redis.
    /// </summary>
    /// <param name="input">The value to encode.</param>
    /// <returns>The encoded value.</returns>
    private string Encode(string input)
    {
        var encoded = HttpUtility.UrlEncode(input);
        return encoded;
    }

    /// <summary>
    /// Decodes a value, for reading from Redis.
    /// </summary>
    /// <param name="input">The value to decode.</param>
    /// <returns>The decoded value.</returns>
    private string Decode(string input)
    {
        var decoded = HttpUtility.UrlDecode(input);
        return decoded;
    }
    #endregion Encoding

    #region Serialization
    /// <summary>
    /// Serializes an entity into Redis hash key-value pairs.
    /// </summary>
    /// <param name="entity">The entity to serialize.</param>
    /// <returns>The serialized entity.</returns>
    private HashEntry[] Serialize(TEntity entity)
    {
        var propertyValues = _properties
            .Select(e => new HashEntry(
                e.Name.ToSnakeCase(),
                JsonSerializer.Serialize(e.GetValue(entity)))
            )
            .ToArray();

        return propertyValues;
    }

    /// <summary>
    /// Deserializes an entity from Redis hash values. Relies on <see cref="_properties"/> being sorted consistently.
    /// </summary>
    /// <param name="data">The data to deserialize.</param>
    /// <returns>The deserialized entity.</returns>
    private TEntity Deserialize(IDatabase database, RedisValue[] data, ConcurrentDictionary<string, object> savedObjects)
    {
        var interceptor = GetInterceptor(database, savedObjects);
        var entity = RedisHelperProxy.Generator.CreateClassProxy<TEntity>(interceptor);

        var values = new Dictionary<string, object?>();

        int index = 0;
        foreach (var property in _properties)
        {
            var propertyValue = JsonSerializer.Deserialize(data[index].ToString(), property.Type);
            property.SetValue(entity, propertyValue);
            values.Add(property.Name, propertyValue);

            index++;
        }

        interceptor.SetInitialValues(values);
        interceptor.Enable();

        return entity;
    }

    /// <summary>
    /// Deserializes a set of entities from Redis hash values. Relies on <see cref="_properties"/> being sorted consistently.
    /// </summary>
    /// <param name="data">The data to deserialize.</param>
    /// <returns>The deserialized entities.</returns>
    private List<TEntity> DeserializeSet(IDatabase database, RedisValue[] data, ConcurrentDictionary<string, object> savedObjects)
    {
        var entities = new List<TEntity>();
        var chunks = data.Chunk(_properties.Count());

        foreach (var chunk in chunks)
        {
            var entity = Deserialize(database, chunk, savedObjects);
            entities.Add(entity);
        }

        return entities;
    }
    #endregion Serialization

    #region Actions
    /// <summary>
    /// Constructs an action to set an entity in Redis.
    /// </summary>
    /// <param name="key">The key of the entity being set.</param>
    /// <param name="entity">The entity to set.</param>
    /// <returns>The set action.</returns>
    public void AppendSetAction(IDatabase database, ITransaction transaction, EntityCacheKey<TEntity> key, TEntity entity)
    {
        var entityKey = GetEntityKey(key);
        var recordData = Serialize(entity);

        _ = transaction.HashSetAsync(entityKey, recordData);
    }

    /// <summary>
    /// Constructs an action to delete an entity in Redis.
    /// </summary>
    /// <param name="key">The key of the entity being deleted.</param>
    /// <returns>The delete action.</returns>
    public void AppendDeleteAction(IDatabase database, ITransaction transaction, EntityCacheKey<TEntity> key)
    {
        var entityKey = GetEntityKey(key);

        _ = transaction.KeyDeleteAsync(entityKey);
    }

    /// <summary>
    /// Constructs an action to read an entity from Redis.
    /// </summary>
    /// <param name="key">The key of the entity being read.</param>
    /// <returns>The read action.</returns>
    public Func<Task<TEntity?>> AppendReadAction(IDatabase database, ITransaction transaction, EntityCacheKey<TEntity> key, ConcurrentDictionary<string, object>? savedObjects = null)
    {
        var entityKey = GetEntityKey(key);
        var action = AppendReadAction(database, transaction, entityKey, savedObjects);
        return action;
    }

    /// <summary>
    /// Constructs an action to read an entity from Redis.
    /// </summary>
    /// <param name="key">The key of the entity being read.</param>
    /// <returns>The read action.</returns>
    internal Func<Task<TEntity?>> AppendReadAction(IDatabase database, ITransaction transaction, RedisKey key, ConcurrentDictionary<string, object>? savedObjects = null)
    {
        savedObjects ??= new ConcurrentDictionary<string, object>();

        var properties = GetReadProperties();

        var valuesTask = transaction.HashGetAsync(key, properties);

        return async () =>
        {
            var values = await valuesTask;
            if (values.Any(e => e.ToString() is not null))
            {
                var entity = Deserialize(database, values, savedObjects);
                return entity;
            }

            return default;
        };
    }

    /// <summary>
    /// Constructs an action to add an entity to a set in Redis.
    /// </summary>
    /// <param name="entity">The entity being added.</param>
    /// <returns>The add action.</returns>
    public void AppendAddAction(IDatabase database, ITransaction transaction, TEntity entity, ConcurrentDictionary<string, object>? savedObjects = null)
    {
        savedObjects ??= new ConcurrentDictionary<string, object>();

        BuildAddAction(database, transaction, entity, savedObjects);
    }

    private void BuildAddAction(IDatabase database, ITransaction transaction, TEntity entity, ConcurrentDictionary<string, object> savedObjects)
    {
        var setKey = GetSetEntityKey(entity);
        var recordData = Serialize(entity);

        var primaryKey = GetEntitySetPrimaryKey();
        var id = GetRecordId(entity);

        _ = transaction.HashSetAsync(setKey, recordData);
        _ = transaction.SortedSetAddAsync(primaryKey, id, 0);

        foreach (var index in _indexes)
        {
            var indexKey = GetEntitySetIndexKey(index.Item1);
            var indexValue = index.Item1.GetValue(entity);
            var indexScore = indexValue!.TryGetScore();
            _ = transaction.SortedSetAddAsync(indexKey, id, indexScore);
        }

        foreach (var lookup in _lookupSingles)
        {
            var maybeSetLookup = GetEntitySetLookup(lookup.Item1, entity);
            if (maybeSetLookup != null)
            {
                var (indexKey, indexValue) = maybeSetLookup.Value;
                _ = transaction.SortedSetAddAsync(indexKey, indexValue, 0);
            }

            var lookupValue = lookup.Item1.GetValue(entity);
            if (lookupValue is not null)
            {
                var lookupType = lookupValue.GetType();
                var helper = typeof(RedisHelper<TEntity>)
                    .GetMethod(nameof(BuildAddActionLookup), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(lookupType);

                helper.Invoke(this, new object[] { database, transaction, lookupValue, savedObjects });
            }
        }

        foreach (var lookup in _lookupSets)
        {
            var lookupValue = lookup.Item1.GetValue(entity) as IEnumerable<object>;
            if (lookupValue is not null)
            {
                var lookupType = lookupValue.GetType().GenericTypeArguments[0];
                var helper = typeof(RedisHelper<TEntity>)
                    .GetMethod(nameof(BuildAddActionLookup), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(lookupType);

                foreach (var value in lookupValue)
                {
                    helper.Invoke(this, new object[] { database, transaction, value, savedObjects });
                }
            }
        }
    }

    private void BuildAddActionLookup<TSubEntity>(IDatabase database, ITransaction transaction, TSubEntity entity, ConcurrentDictionary<string, object> savedObjects)
        where TSubEntity : class, new()
    {
        var helper = _factory.CreateHelper<TSubEntity>();

        var entityKey = helper.GetSetEntityKey(entity);

        //Only write the record to Redis if it wasn't written already
        if (savedObjects.TryAdd(entityKey.ToString(), entity!))
        {
            helper.BuildAddAction(database, transaction, entity, savedObjects);
        }
    }

    /// <summary>
    /// Constructs an action to remove an entity from a set in Redis.
    /// </summary>
    /// <param name="entity">The entity being removed.</param>
    /// <returns>The remove action.</returns>
    public void AppendRemoveAction(IDatabase database, ITransaction transaction, TEntity entity)
    {
        var setKey = GetSetEntityKey(entity);

        var primaryKey = GetEntitySetPrimaryKey();
        var id = GetRecordId(entity);

        _ = transaction.KeyDeleteAsync(setKey);
        _ = transaction.SortedSetRemoveAsync(primaryKey, id);

        foreach (var index in _indexes)
        {
            var indexKey = GetEntitySetIndexKey(index.Item1);
            _ = transaction.SortedSetRemoveAsync(indexKey, id);
        }
    }

    /// <summary>
    /// Constructs an action to list entities from a set in Redis.
    /// </summary>
    /// <param name="filters">The filters to apply to the set.</param>
    /// <returns>The list action.</returns>
    public Func<Task<List<TEntity>>> AppendListAction(
        IDatabase database,
        ITransaction transaction,
        IEnumerable<ICacheEntitySetFilter<TEntity>>? filters = null,
        RedisKey? lookupKey = null,
        ConcurrentDictionary<string, object>? savedObjects = null)
    {
        if (filters is not null && lookupKey is not null)
            throw new ArgumentException($"May only specify either {nameof(filters)} or {nameof(lookupKey)}");

        filters ??= Enumerable.Empty<ICacheEntitySetFilter<TEntity>>();
        savedObjects ??= new ConcurrentDictionary<string, object>();

        var primaryKey = GetEntitySetPrimaryKey();
        var properties = GetListProperties();

        var useKeyDelete = false;
        var useKey = lookupKey ?? primaryKey;

        if (filters.Any())
        {
            var allTempKeys = new List<RedisKey>();
            foreach (var filter in filters)
            {
                var indexProperty = filter.GetProperty();
                var indexKey = GetEntitySetIndexKey(indexProperty);

                var startScore = filter.GetStartScore();
                var endScore = filter.GetEndScore();
                var exclusivity =
                    filter.GetStartInclusivity() && filter.GetEndInclusivity() ? Exclude.None
                    : filter.GetStartInclusivity() ? Exclude.Stop
                    : filter.GetEndInclusivity() ? Exclude.Start
                    : Exclude.Both;

                var tempKey = GetTempKey();
                allTempKeys.Add(tempKey);

                _ = transaction.SortedSetRangeAndStoreAsync(
                    indexKey,
                    tempKey,
                    startScore,
                    endScore,
                    exclude: exclusivity,
                    sortedSetOrder: SortedSetOrder.ByScore);
            }

            var finalTempKey = GetTempKey();
            _ = transaction.SortedSetCombineAndStoreAsync(SetOperation.Intersect, finalTempKey, allTempKeys.ToArray());
            _ = transaction.KeyDeleteAsync(allTempKeys.ToArray());

            useKeyDelete = true;
            useKey = finalTempKey;
        }

        var entitiesTask = BuildListAction(database, transaction, useKey, useKeyDelete, savedObjects);

        return async () =>
        {
            return await entitiesTask();
        };
    }

    private Func<Task<List<TEntity>>> BuildListAction(IDatabase database, ITransaction transaction, RedisKey listKey, bool deleteListKey, ConcurrentDictionary<string, object> savedObjects)
    {
        var properties = GetListProperties();

        var valuesTask = transaction.SortAsync(
            listKey,
            sortType: SortType.Numeric,
            by: "nosort",
            get: properties);

        if (deleteListKey)
        {
            _ = transaction.KeyDeleteAsync(listKey);
        }

        var action = async () =>
        {
            var values = await valuesTask;

            var entities = DeserializeSet(database, values, savedObjects);

            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var entityKey = GetRecordId(entity).ToString();
                if (!savedObjects.TryAdd(entityKey, entity!))
                {
                    entities[i] = (TEntity)savedObjects[entityKey];
                }
            }

            return entities;
        };

        return action;
    }

    private RedisLazyLoadingInterceptor<TEntity> GetInterceptor(IDatabase database, ConcurrentDictionary<string, object> savedObjects)
    {
        var interceptor = new RedisLazyLoadingInterceptor<TEntity>(database, savedObjects, _keys, _properties, _lookupSingles, _lookupSets);

        return interceptor;
    }
    #endregion Actions

    public string GetTableName()
    {
        return _tableKey;
    }
}
