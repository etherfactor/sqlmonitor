using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Extensions;
using StackExchange.Redis;
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
    private readonly string _tableKey;
    private readonly IEnumerable<(PropertyInfo, ColumnAttribute)> _keys;
    private readonly IEnumerable<(PropertyInfo, ColumnAttribute)> _indexes;
    private readonly IEnumerable<(PropertyInfo, ColumnAttribute)> _properties;

    /// <summary>
    /// Use <see cref="RedisHelperCache.For{TEntity}"/> instead!
    /// </summary>
    public RedisHelper()
    {
        var tableAttribute = typeof(TEntity).GetCustomAttribute<TableAttribute>()
            ?? throw new InvalidOperationException($"To cache {typeof(TEntity).Name}, it must be annotated with a {nameof(TableAttribute)}.");
        _tableKey = tableAttribute.Name;

        var allProperties = typeof(TEntity)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public);

        var clrProperties = allProperties
            .Where(x => x.PropertyType.IsSerializable);

        var sortedProperties = clrProperties
            .Select(e =>
            {
                var Property = e;
                var Attribute = e.GetCustomAttribute<ColumnAttribute>()!;
                return (Property, Attribute);
            })
            .Where(e => e.Attribute is not null)
            .OrderBy(e => e.Property.Name).ToList();

        _keys = sortedProperties
            .Where(e => e.Property.GetCustomAttribute<KeyAttribute>() is not null).ToList();

        _properties = sortedProperties
            .Where(e => e.Property.GetCustomAttribute<KeyAttribute>() is null).ToList();

        _indexes = sortedProperties
            .Where(e => e.Property.GetCustomAttribute<IndexedAttribute>() is not null).ToList();
    }

    private HashEntry[] Serialize(TEntity entity)
    {
        var propertyValues = _properties.Select(e => new HashEntry(
            e.Item1.Name.ToSnakeCase(),
            JsonSerializer.Serialize(e.Item1.GetValue(entity))))
            .ToArray();

        return propertyValues;
    }

    private List<TEntity> Deserialize(RedisValue[] data)
    {
        var entities = new List<TEntity>();
        var chunks = data.Chunk(_properties.Count() + 1);

        foreach (var chunk in chunks)
        {
            var entity = new TEntity();
            entities.Add(entity);

            var materializedChunk = chunk.ToList();

            var keyData = materializedChunk[0].ToString();
            var keyRawValues = keyData.Split('|');

            int index = 0;
            foreach (var tuple in _keys)
            {
                var key = tuple.Item1;
                var keyValue = JsonSerializer.Deserialize(Decode(keyRawValues[index]), key.PropertyType);
                key.SetValue(entity, keyValue);

                index++;
            }

            index = 1;
            foreach (var tuple in _properties)
            {
                var property = tuple.Item1;
                var propertyValue = JsonSerializer.Deserialize(materializedChunk[index].ToString(), property.PropertyType);
                property.SetValue(entity, propertyValue);

                index++;
            }
        }

        return entities;
    }

    public RedisKey GetSetEntityKey(TEntity entity)
    {
        var raw = _keys.Select(e => JsonSerializer.Serialize(e.Item1.GetValue(entity)))
            .Select(e => Encode(e));
        var value = new RedisKey($"{_tableKey}:{string.Join("|", raw)}");

        return value;
    }

    private RedisKey GetEntitySetPrimaryKey()
    {
        var keyData = new RedisKey($"{_tableKey}:$$primary");
        return keyData;
    }

    private RedisValue GetRecordId(TEntity entity)
    {
        var raw = _keys.Select(e => JsonSerializer.Serialize(e.Item1.GetValue(entity)))
            .Select(e => Encode(e));
        var value = new RedisValue(string.Join("|", raw));

        return value;
    }

    private RedisKey GetRecordKey(TEntity entity)
    {
        var raw = GetRecordId(entity);
        var value = new RedisKey($"{_tableKey}:{raw}");

        return value;
    }

    private RedisKey GetEntityKey(EntityCacheKey<TEntity> key)
    {
        var keyData = new RedisKey($"{Constants.CacheSchemaName}:$$entity:{key.KeyName}");
        return keyData;
    }

    private RedisValue[] GetProperties()
    {
        var propertyNames = "#".Yield()
            .Concat(_properties.Select(e => $"{_tableKey}:*->{e.Item1.Name.ToSnakeCase()}"))
            .Select(e => new RedisValue(e))
            .ToArray();

        return propertyNames;
    }

    private string Encode(string input)
    {
        var encoded = HttpUtility.UrlEncode(input);
        return encoded;
    }

    private string Decode(string input)
    {
        var decoded = HttpUtility.UrlDecode(input);
        return decoded;
    }

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

    public Func<IDatabase, Task> GetDeleteAction(EntityCacheKey<TEntity> key)
    {
        var entityKey = GetEntityKey(key);

        var action = async (IDatabase database) =>
        {
            await database.KeyDeleteAsync(entityKey);
        };

        return action;
    }

    public Func<IDatabase, Task> GetAddAction(TEntity entity)
    {
        var setKey = GetSetEntityKey(entity);
        var recordData = Serialize(entity);

        var primaryKey = GetEntitySetPrimaryKey();
        var id = GetRecordId(entity);

        var action = async (IDatabase database) =>
        {
            var transaction = database.CreateTransaction();
            _ = transaction.HashSetAsync(setKey, recordData);
            _ = transaction.SortedSetAddAsync(primaryKey, id, 0);

            foreach (var index in _indexes)
            {
                var indexKey = GetEntitySetIndexKey(index.Item1);
                var indexValue = index.Item1.GetValue(entity);
                var indexScore = indexValue!.TryGetScore();
                _ = transaction.SortedSetAddAsync(indexKey, id, indexScore);
            }

            await transaction.ExecuteAsync();
        };

        return action;
    }

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

    public Func<IDatabase, Task<List<TEntity>>> GetListAction(IEnumerable<CacheEntitySetFilter<TEntity>> filters)
    {
        var primaryKey = GetEntitySetPrimaryKey();
        var properties = GetProperties();

        if (filters.Any())
        {
            var action = async (IDatabase database) =>
            {
                var transaction = database.CreateTransaction();

                var allTempKeys = new List<RedisKey>();
                foreach (var filter in filters)
                {
                    var indexProperty = filter.GetProperty();
                    var indexKey = GetEntitySetIndexKey(indexProperty);

                    var startScore = filter.GetStartScore();
                    var endScore = filter.GetEndScore();
                    var exclusivity = filter.GetExclusivity();

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
                var valuesTask = transaction.SortAsync(
                    finalTempKey,
                    sortType: SortType.Numeric,
                    by: "nosort",
                    get: properties);

                await transaction.ExecuteAsync();
                var values = await valuesTask;

                var entities = Deserialize(values);
                return entities;
            };

            return action;
        }
        else
        {
            var action = async (IDatabase database) =>
            {
                var values = await database.SortAsync(
                    primaryKey,
                    sortType: SortType.Numeric,
                    by: "nosort",
                    get: properties);

                var entities = Deserialize(values);
                return entities;
            };

            return action;
        }
    }

    private RedisKey GetEntitySetIndexKey(PropertyInfo index)
    {
        var indexAttribute = index.GetCustomAttribute<IndexedAttribute>()
            ?? throw new InvalidOperationException($"Can only get an index for an indexed property, specifying an {nameof(IndexedAttribute)}.");

        var keyData = new RedisKey($"{_tableKey}:${indexAttribute.Name.ToSnakeCase()}");
        return keyData;
    }

    private RedisKey GetTempKey()
    {
        var guid = Guid.NewGuid();
        var tempKey = new RedisKey($"sqlpulse:$$temp:{guid}");
        return tempKey;
    }
}
