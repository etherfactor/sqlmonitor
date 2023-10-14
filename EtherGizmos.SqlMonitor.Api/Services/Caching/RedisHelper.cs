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

/// <summary>
/// Provides helper methods for working with Redis caching.
/// </summary>
/// <typeparam name="TEntity">The type of entity being cached.</typeparam>
public class RedisHelper<TEntity>
    where TEntity : new()
{
    private const char IdSeparator = '|';

    private readonly string _tableKey;
    private readonly IEnumerable<(PropertyInfo, ColumnAttribute)> _keys;
    private readonly IEnumerable<(PropertyInfo, ColumnAttribute)> _indexes;
    private readonly IEnumerable<(PropertyInfo, LookupSingleAttribute)> _lookupSingles;
    private readonly IEnumerable<(PropertyInfo, ColumnAttribute)> _lookupSets;
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
                var Attribute = e.GetCustomAttribute<LookupSingleAttribute>()!;
                return (Property, Attribute);
            })
            .Where(e => e.Attribute is not null)
            .ToList();
        ValidateLookupSingles();

        //Lookups define a LookupSetAttribute
        _lookupSets = sortedProperties
            .Where(e => e.Property.GetCustomAttribute<LookupSetAttribute>() is not null)
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
        var raw = _keys.Select(e => JsonSerializer.Serialize(e.Item1.GetValue(entity)))
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
        var lookupAttribute = lookup.GetCustomAttribute<LookupSetAttribute>()
            ?? throw new InvalidOperationException($"Can only get a lookup for a lookup property, specifying a {nameof(LookupSetAttribute)}.");

        var lookupData = new RedisKey($"{Constants.Cache.SchemaName}:$$table:{_tableKey}:${lookup.GetCustomAttribute<ColumnAttribute>()!.Name!.ToSnakeCase()}");
        return lookupData;
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
    private TEntity Deserialize(RedisValue[] data)
    {
        var entity = new TEntity();

        var keyData = data[0].ToString();
        var keyRawValues = keyData.Split(IdSeparator);

        int index = 0;
        foreach (var tuple in _keys)
        {
            var key = tuple.Item1;
            var keyValue = JsonSerializer.Deserialize(Decode(keyRawValues[index]), key.PropertyType);
            if (key.CanWrite)
                key.SetValue(entity, keyValue);

            index++;
        }

        index = 1;
        foreach (var tuple in _properties)
        {
            var property = tuple.Item1;
            var propertyValue = JsonSerializer.Deserialize(data[index].ToString(), property.PropertyType);
            if (property.CanWrite)
                property.SetValue(entity, propertyValue);

            index++;
        }

        return entity;
    }

    /// <summary>
    /// Deserializes a set of entities from Redis hash values. Relies on <see cref="_properties"/> being sorted consistently.
    /// </summary>
    /// <param name="data">The data to deserialize.</param>
    /// <returns>The deserialized entities.</returns>
    private List<TEntity> DeserializeSet(RedisValue[] data)
    {
        var entities = new List<TEntity>();
        var chunks = data.Chunk(_properties.Count() + 1);

        foreach (var chunk in chunks)
        {
            var entity = Deserialize(chunk);
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
    public Func<IDatabase, Task> GetSetAction(EntityCacheKey<TEntity> key, TEntity entity)
    {
        var entityKey = GetEntityKey(key);
        var recordData = Serialize(entity);

        var action = async (IDatabase database) =>
        {
            await database.HashSetAsync(entityKey, recordData);
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
    public Func<IDatabase, Task<TEntity?>> GetReadAction(EntityCacheKey<TEntity> key)
    {
        var entityKey = GetEntityKey(key);
        var properties = GetProperties();

        var action = async (IDatabase database) =>
        {
            var values = await database.HashGetAsync(entityKey, properties);
            if (values.Any(e => e.ToString() is not null))
            {
                var entity = Deserialize(values);
                return entity;
            }

            return default;
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
            var lookupValue = lookup.Item1.GetValue(entity);
            if (lookupValue is not null)
            {
                var lookupType = lookupValue.GetType();
                var helper = typeof(RedisHelper<TEntity>)
                    .GetMethod(nameof(BuildAddActionLookupSingle), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(lookupType);

                helper.Invoke(this, new object[] { lookupValue, transaction, savedObjects });
            }
        }

        foreach (var lookup in _lookupSets)
        {
            var lookupKey = GetEntitySetLookupSetKey(lookup.Item1);
            var lookupValue = lookup.Item1.GetValue(entity) as IEnumerable<object>;
            if (lookupValue is not null)
            {
                var lookupType = lookupValue.GetType().GenericTypeArguments[0];
                var helper = typeof(RedisHelper<TEntity>)
                    .GetMethod(nameof(BuildAddActionLookupSet), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(lookupType);

                helper.Invoke(this, new object[] { lookupValue, lookupKey, transaction, savedObjects });
            }
        }
    }

    private void BuildAddActionLookupSingle<TSubEntity>(TSubEntity entity, ITransaction transaction, ConcurrentDictionary<string, object> savedObjects)
        where TSubEntity : new()
    {
        var helper = RedisHelperCache.For<TSubEntity>();

        var entityKey = helper.GetSetEntityKey(entity);

        //Only write the record to Redis if it wasn't written already
        if (savedObjects.TryAdd(entityKey.ToString(), entity!))
        {
            helper.BuildAddAction(entity, transaction, savedObjects);
        }
    }

    private void BuildAddActionLookupSet<TSubEntity>(IEnumerable<TSubEntity> entities, RedisKey lookupKey, ITransaction transaction, ConcurrentDictionary<string, object> savedObjects)
        where TSubEntity : new()
    {
        var helper = RedisHelperCache.For<TSubEntity>();
        foreach (var entity in entities)
        {
            var entityKey = helper.GetSetEntityKey(entity);

            //Only write the record to Redis if it wasn't written already
            if (savedObjects.TryAdd(entityKey.ToString(), entity!))
            {
                var id = helper.GetRecordId(entity);
                _ = transaction.SortedSetAddAsync(lookupKey, id, 0);
                helper.BuildAddAction(entity, transaction, savedObjects);
            }
        }
    }

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
    public Func<IDatabase, Task<List<TEntity>>> GetListAction(IEnumerable<ICacheEntitySetFilter<TEntity>> filters)
    {
        var primaryKey = GetEntitySetPrimaryKey();
        var properties = GetProperties();

        var useKeyDelete = false;
        var useKey = primaryKey;

        var action = async (IDatabase database) =>
        {
            var transaction = database.CreateTransaction();

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

            var valuesTask = transaction.SortAsync(
                useKey,
                sortType: SortType.Numeric,
                by: "nosort",
                get: properties);

            if (useKeyDelete)
            {
                _ = transaction.KeyDeleteAsync(useKey);
            }

            await transaction.ExecuteAsync();
            var values = await valuesTask;

            var entities = DeserializeSet(values);

            foreach (var entity in entities)
            {
                foreach (var lookup in GetLookupKeys(entity))
                {
                    //adjust method to also return property, probably
                    //create a RedisHelperCache.For<TEntity>() on the entity type
                    //use that cache to fetch corresponding lookup records
                }
            }
            return entities;
        };

        return action;
    }
    #endregion Actions
}
