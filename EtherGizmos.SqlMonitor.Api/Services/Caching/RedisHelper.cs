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
public class RedisHelper<TEntity>
    where TEntity : class, new()
{
    private const char IdSeparator = '|';

    private readonly string _tableKey;
    private readonly IEnumerable<(PropertyInfo, ColumnAttribute)> _all;
    private readonly IEnumerable<(PropertyInfo, ColumnAttribute)> _keys;
    private readonly IEnumerable<(PropertyInfo, ColumnAttribute)> _indexes;
    private readonly IEnumerable<(PropertyInfo, LookupAttribute)> _lookupSingles;
    private readonly IEnumerable<(PropertyInfo, LookupIndexAttribute)> _lookupSets;
    private readonly IEnumerable<(PropertyInfo, ColumnAttribute)> _properties;

    /// <summary>
    /// Use <see cref="RedisHelperCache.For{TEntity}"/> instead!
    /// </summary>
    public RedisHelper()
    {
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
                var Property = e;
                var Attribute = e.GetCustomAttribute<ColumnAttribute>()!;
                return (Property, Attribute);
            })
            .Where(e => e.Attribute is not null)
            .OrderBy(e => e.Attribute.Name).ToList();
        _all = sortedProperties;

        //Keys define a KeyAttribute
        _keys = sortedProperties
            .Where(e => e.Property.GetCustomAttribute<KeyAttribute>() is not null)
            .ToList();
        ValidateKeys();

        //While properties do not; keys do not need to be duplicated where properties are stored
        _properties = sortedProperties
            .Where(e => e.Property.GetCustomAttribute<KeyAttribute>() is null)
            .Where(e => !e.Property.PropertyType.IsComplexType())
            .ToList();

        //Indexes define an IndexedAttribute
        _indexes = sortedProperties
            .Where(e => e.Property.GetCustomAttribute<IndexedAttribute>() is not null)
            .ToList();
        ValidateIndexes();

        //Lookups define a LookupSingleAttribute
        _lookupSingles = allProperties
            .Select(e =>
            {
                var Property = e;
                var Attribute = e.GetCustomAttribute<LookupAttribute>()!;
                return (Property, Attribute);
            })
            .Where(e => e.Attribute is not null)
            .ToList();
        ValidateLookupSingles();

        //Lookups define a LookupSetAttribute
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
        return GetRecordId(_keys.Select(e => e.Item1.GetValue(entity)!));
    }

    private RedisValue GetRecordId(IEnumerable<object> keys)
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
                .Select(e => _all.Single(p => p.Item1.Name == e))
                .OrderBy(e => e.Item2.Name);

            var maybeLookupValues = lookupProperties
                .Select(e => e.Item1.GetValue(entity));

            if (maybeLookupValues.All(e => e is not null))
            {
                var lookupValues = maybeLookupValues.Select(e => JsonSerializer.Serialize(e))
                    .Select(e => Encode(e));

                var lookupTableKey = lookup.PropertyType.GetCustomAttribute<TableAttribute>()!.Name;

                var redisKey = new RedisKey($"{Constants.Cache.SchemaName}:$$table:{_tableKey}:{lookupTableKey}:${lookupIndexAttribute.Name.ToSnakeCase()}");
                var redisValue = new RedisValue(string.Join(IdSeparator, lookupValues));

                return (redisKey, redisValue);
            }
        }
        else if (lookupAttribute.Record is not null)
        {

        }

        return null;
    }

    /// <summary>
    /// Generate a temporary key.
    /// </summary>
    /// <returns>The temporary key.</returns>
    private RedisKey GetTempKey()
    {
        var guid = Guid.NewGuid();
        var tempKey = new RedisKey($"{Constants.Cache.SchemaName}:$$temp:{guid}");
        return tempKey;
    }

    /// <summary>
    /// Constructs the Redis properties for the entity.
    /// </summary>
    /// <returns>The Redis properties.</returns>
    private RedisValue[] GetProperties()
    {
        var propertyNames = "#".Yield()
            .Concat(_properties.Select(e => $"{Constants.Cache.SchemaName}:$$table:{_tableKey}:*->{e.Item1.Name.ToSnakeCase()}"))
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
            .Where(e => e.Item1.CanRead)
            .Select(e => new HashEntry(
            e.Item1.Name.ToSnakeCase(),
            JsonSerializer.Serialize(e.Item1.GetValue(entity))))
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

        var keyData = data[0].ToString();
        var keyRawValues = keyData.Split(IdSeparator);

        int index = 0;
        foreach (var tuple in _keys)
        {
            var key = tuple.Item1;
            var keyValue = JsonSerializer.Deserialize(Decode(keyRawValues[index]), key.PropertyType);
            if (key.CanWrite)
            {
                key.SetValue(entity, keyValue);
                values.Add(key.Name, keyValue);
            }

            index++;
        }

        index = 1;
        foreach (var tuple in _properties)
        {
            var property = tuple.Item1;
            var propertyValue = JsonSerializer.Deserialize(data[index].ToString(), property.PropertyType);
            if (property.CanWrite)
            {
                property.SetValue(entity, propertyValue);
                values.Add(property.Name, propertyValue);
            }

            index++;
        }

        interceptor.SetInitialValues(values);

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
        var chunks = data.Chunk(_properties.Count() + 1);

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
    public Func<IDatabase, Func<Task>> GetSetActionBuilder(EntityCacheKey<TEntity> key, TEntity entity, ITransaction? transaction = null)
    {
        var entityKey = GetEntityKey(key);
        var recordData = Serialize(entity);

        var action = (IDatabase database) =>
        {
            transaction ??= database.CreateTransaction();

            return async () =>
            {
                await database.HashSetAsync(entityKey, recordData);
            };
        };

        return action;
    }

    /// <summary>
    /// Constructs an action to delete an entity in Redis.
    /// </summary>
    /// <param name="key">The key of the entity being deleted.</param>
    /// <returns>The delete action.</returns>
    public Func<IDatabase, Task> GetDeleteAction(EntityCacheKey<TEntity> key)
    {
        var entityKey = GetEntityKey(key);

        var action = async (IDatabase database) =>
        {
            await database.KeyDeleteAsync(entityKey);
        };

        return action;
    }

    /// <summary>
    /// Constructs an action to read an entity from Redis.
    /// </summary>
    /// <param name="key">The key of the entity being read.</param>
    /// <returns>The read action.</returns>
    public Func<IDatabase, Func<Task<TEntity?>>> GetReadActionBuilder(EntityCacheKey<TEntity> key, ConcurrentDictionary<string, object>? savedObjects = null)
    {
        var entityKey = GetEntityKey(key);
        var action = GetReadActionBuilder(entityKey, savedObjects);
        return action;
    }

    /// <summary>
    /// Constructs an action to read an entity from Redis.
    /// </summary>
    /// <param name="key">The key of the entity being read.</param>
    /// <returns>The read action.</returns>
    public Func<IDatabase, Func<Task<TEntity?>>> GetReadActionBuilder(RedisKey key, ConcurrentDictionary<string, object>? savedObjects = null)
    {
        savedObjects ??= new ConcurrentDictionary<string, object>();

        var properties = GetProperties();

        var action = (IDatabase database) =>
        {
            return async () =>
            {
                var values = await database.HashGetAsync(key, properties);
                if (values.Any(e => e.ToString() is not null))
                {
                    var entity = Deserialize(database, values, savedObjects);
                    return entity;
                }

                return default;
            };
        };

        return action;
    }

    /// <summary>
    /// Constructs an action to add an entity to a set in Redis.
    /// </summary>
    /// <param name="entity">The entity being added.</param>
    /// <returns>The add action.</returns>
    public Func<IDatabase, Task> GetAddAction(TEntity entity)
    {
        var action = async (IDatabase database) =>
        {
            var transaction = database.CreateTransaction();

            BuildAddAction(entity, transaction, new ConcurrentDictionary<string, object>());

            await transaction.ExecuteAsync();
        };

        return action;
    }

    private void BuildAddAction(TEntity entity, ITransaction transaction, ConcurrentDictionary<string, object> savedObjects)
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

                helper.Invoke(this, new object[] { lookupValue, transaction, savedObjects });
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
                    helper.Invoke(this, new object[] { value, transaction, savedObjects });
                }
            }
        }
    }

    private void BuildAddActionLookup<TSubEntity>(TSubEntity entity, ITransaction transaction, ConcurrentDictionary<string, object> savedObjects)
        where TSubEntity : class, new()
    {
        var helper = RedisHelperCache.For<TSubEntity>();

        var entityKey = helper.GetSetEntityKey(entity);

        //Only write the record to Redis if it wasn't written already
        if (savedObjects.TryAdd(entityKey.ToString(), entity!))
        {
            helper.BuildAddAction(entity, transaction, savedObjects);
        }
    }

    //private void BuildAddActionLookupSet<TSubEntity>(IEnumerable<TSubEntity> entities, ITransaction transaction, ConcurrentDictionary<string, object> savedObjects)
    //    where TSubEntity : new()
    //{
    //    var helper = RedisHelperCache.For<TSubEntity>();
    //    foreach (var entity in entities)
    //    {
    //        var entityKey = helper.GetSetEntityKey(entity);

    //        //Only write the record to Redis if it wasn't written already
    //        if (savedObjects.TryAdd(entityKey.ToString(), entity!))
    //        {
    //            helper.BuildAddAction(entity, transaction, savedObjects);
    //        }
    //    }
    //}

    /// <summary>
    /// Constructs an action to remove an entity from a set in Redis.
    /// </summary>
    /// <param name="entity">The entity being removed.</param>
    /// <returns>The remove action.</returns>
    public Func<IDatabase, Task> GetRemoveAction(TEntity entity)
    {
        var setKey = GetSetEntityKey(entity);
        var recordData = Serialize(entity);

        var primaryKey = GetEntitySetPrimaryKey();
        var id = GetRecordId(entity);

        var action = async (IDatabase database) =>
        {
            var transaction = database.CreateTransaction();
            _ = transaction.KeyDeleteAsync(setKey);
            _ = transaction.SortedSetRemoveAsync(primaryKey, id);

            foreach (var index in _indexes)
            {
                var indexKey = GetEntitySetIndexKey(index.Item1);
                _ = transaction.SortedSetRemoveAsync(indexKey, id);
            }

            await transaction.ExecuteAsync();
        };

        return action;
    }

    /// <summary>
    /// Constructs an action to list entities from a set in Redis.
    /// </summary>
    /// <param name="filters">The filters to apply to the set.</param>
    /// <returns>The list action.</returns>
    public Func<IDatabase, Func<Task<List<TEntity>>>> GetListActionBuilder(
        IEnumerable<ICacheEntitySetFilter<TEntity>>? filters = null,
        RedisKey? lookupKey = null,
        ConcurrentDictionary<string, object>? savedObjects = null,
        ITransaction? transaction = null)
    {
        if (filters is not null && lookupKey is not null)
            throw new ArgumentException($"May only specify either {nameof(filters)} or {nameof(lookupKey)}");

        filters ??= Enumerable.Empty<ICacheEntitySetFilter<TEntity>>();

        var primaryKey = GetEntitySetPrimaryKey();
        var properties = GetProperties();

        var useKeyDelete = false;
        var useKey = primaryKey;

        var buildTransaction = (IDatabase database) =>
        {
            var thisSavedObjects = savedObjects
                ?? new ConcurrentDictionary<string, object>();

            transaction ??= database.CreateTransaction();

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
                        sortedSetOrder: SortedSetOrder.ByScore);
                }

                var finalTempKey = GetTempKey();
                _ = transaction.SortedSetCombineAndStoreAsync(SetOperation.Intersect, finalTempKey, allTempKeys.ToArray());
                _ = transaction.KeyDeleteAsync(allTempKeys.ToArray());

                useKeyDelete = true;
                useKey = finalTempKey;
            }

            var entitiesTask = BuildListAction(database, transaction, useKey, useKeyDelete, thisSavedObjects);

            return async () =>
            {
                await transaction.ExecuteAsync();
                return await entitiesTask();
            };
        };

        return buildTransaction;
    }

    private Func<Task<List<TEntity>>> BuildListAction(IDatabase database, ITransaction transaction, RedisKey listKey, bool deleteListKey, ConcurrentDictionary<string, object> savedObjects)
    {
        var properties = GetProperties();

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

    //TODO: abstract the above away, so it passes the key name of a list used to look up entities
    //On a given entity, its metadata can be used to find its lookup list, which can be passed to the same method
    //Allows for recursive record retrieval
    private async Task PopulateLookups(TEntity entity, IDatabase database, ConcurrentDictionary<string, object> savedObjects)
    {
        foreach (var lookup in _lookupSingles)
        {
            var keyProperties = lookup.Item2.IdProperties
                .Select(e => typeof(TEntity).GetProperty(e, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                .Select(e => e!.GetValue(entity)!);

            var key = GetRecordId(keyProperties);
        }

        foreach (var lookup in _lookupSets)
        {
            var lookupType = lookup.Item1.PropertyType.GenericTypeArguments[0];

            var method = typeof(RedisHelper<TEntity>).GetMethod(nameof(PopulateLookupSet), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(lookupType);

            var methodTask = (Task)method.Invoke(this, new object?[] { entity, lookup.Item1, database, savedObjects })!;
            await methodTask;
        }
    }

    //private async Task PopulateLookupSingle<TSubEntity>(TEntity entity, ConcurrentDictionary<string, object> savedObjects)
    //{

    //}

    private async Task PopulateLookupSet<TSubEntity>(TEntity entity, PropertyInfo lookupProperty, IDatabase database, ConcurrentDictionary<string, object> savedObjects)
        where TSubEntity : class, new()
    {
        var helper = RedisHelperCache.For<TSubEntity>();

        var lookupKey = GetEntitySetLookupSetKey(lookupProperty);
        var builder = helper.GetListActionBuilder(lookupKey: lookupKey, savedObjects: savedObjects);

        var action = builder(database);
        var lookupSet = await action();

        lookupProperty.SetValue(entity, lookupSet);
    }

    private RedisLazyLoadingInterceptor<TEntity> GetInterceptor(IDatabase database, ConcurrentDictionary<string, object> savedObjects)
    {
        var interceptor = new RedisLazyLoadingInterceptor<TEntity>(database, savedObjects, _lookupSingles, _lookupSets);

        return interceptor;
    }
    #endregion Actions

    public string GetTableName()
    {
        return _tableKey;
    }
}
