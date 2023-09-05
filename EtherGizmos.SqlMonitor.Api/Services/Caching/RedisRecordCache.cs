using EtherGizmos.SqlMonitor.Api.Extensions;
using MassTransit.Internals;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class RedisRecordCache
{
    private readonly IDatabase _database;

    public RedisRecordCache(IConnectionMultiplexer multiplexer)
    {
        _database = multiplexer.GetDatabase();
    }

    public ICacheEntitySet<TEntity> EntitySet<TEntity>(RecordCacheKey<TEntity> key)
        where TEntity : new()
    {
        return new CacheEntitySet<TEntity>(key, _database);
    }
}

public struct RecordCacheKey<TEntity>
{
    public readonly string Name { get; }

    public RecordCacheKey(string name)
    {
        Name = name;
    }
}

public interface ICacheEntitySet<TEntity> : ICanAlterSet<TEntity>, ICanFilter<TEntity>, ICanList<TEntity>
{
}

public class CacheEntitySet<TEntity> : ICacheEntitySet<TEntity>
    where TEntity : new()
{
    private readonly RecordCacheKey<TEntity> _key;
    private readonly IDatabase _database;

    public CacheEntitySet(RecordCacheKey<TEntity> key, IDatabase database)
    {
        _key = key;
        _database = database;
    }

    public async Task AddAsync(TEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var serializer = RedisSerializerCache.For<TEntity>();
        var action = serializer.GetAddAction(_key, entity);
        await action(_database);
    }

    public async Task RemoveAsync(TEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var serializer = RedisSerializerCache.For<TEntity>();
        var action = serializer.GetRemoveAction(_key, entity);
        await action(_database);
    }

    public async Task<List<TEntity>> ToListAsync()
    {
        var serializer = RedisSerializerCache.For<TEntity>();
        var action = serializer.GetListAction(_key);
        return await action(_database);
    }

    public ICanCompare<TEntity, TProperty> Where<TProperty>(Expression<Func<TEntity, TProperty>> indexedProperty)
    {
        var memberName = indexedProperty.GetMemberName();

        throw new NotImplementedException();
    }
}

public static class CacheObjectExtensions
{
    public static HashEntry[] ToCacheProperties(this object @this)
    {
        var entries = new List<HashEntry>();

        foreach (var property in @this.GetType().GetProperties())
        {
            if (property.CanRead)
            {
                var valueData = property.GetValue(@this);
                var valueString = (string?)Convert.ChangeType(valueData, typeof(string));

                entries.Add(new HashEntry(property.Name.ToSnakeCase(), valueString));
            }
        }

        return entries.ToArray();
    }

    public static RedisValue[] ToCacheKeys(this Type @this)
    {
        var entries = new List<RedisValue>() { "#" };

        foreach (var property in @this.GetProperties())
        {
            if (property.CanRead)
            {
                entries.Add(property.Name.ToSnakeCase());
            }
        }

        return entries.ToArray();
    }

    public static string ToCacheId(this object @this)
    {
        var keyProperties = @this.GetType().GetProperties()
            .Where(e => e.GetCustomAttribute<KeyAttribute>() is not null)
            .OrderBy(e => e.Name);

        var keyId = string.Join(
            "|",
            keyProperties.Select(e => Convert.ChangeType(e.GetValue(@this), typeof(string))));

        return keyId;
    }
}

public interface ICanList<TEntity>
{
    Task<List<TEntity>> ToListAsync();
}

public interface ICanAlterSet<TEntity>
{
    Task AddAsync(TEntity entity);

    Task RemoveAsync(TEntity entity);
}

public interface ICanFilter<TEntity>
{
    ICanCompare<TEntity, TProperty> Where<TProperty>(Expression<Func<TEntity, TProperty>> indexedProperty);
}

public interface ICanCompare<TEntity, TProperty>
{
    ICanFilter<TEntity> IsEqualTo(TProperty value);

    ICanFilter<TEntity> IsGreaterThan(TProperty value);

    ICanFilter<TEntity> IsGreaterThanOrEqualTo(TProperty value);

    ICanFilter<TEntity> IsLessThan(TProperty value);

    ICanFilter<TEntity> IsLessThanOrEqualTo(TProperty value);

    ICanFilter<TEntity> IsBetween(TProperty valueStart, TProperty valueEnd);
}

public static class ICanCompareExtensions
{
    public static ICanFilter<TEntity> StartsWith<TEntity>(this ICanCompare<TEntity, string> @this)
    {
        throw new NotImplementedException();
    }
}

public static class RedisSerializerCache
{
    private static readonly IDictionary<Type, object> _serializers = new Dictionary<Type, object>();

    public static RedisSerializer<TEntity> For<TEntity>()
        where TEntity : new()
    {
        if (!_serializers.ContainsKey(typeof(TEntity)))
        {
            var serializer = new RedisSerializer<TEntity>();
            _serializers.Add(typeof(TEntity), serializer);
        }

        return (RedisSerializer<TEntity>)_serializers[typeof(TEntity)];
    }
}

public class RedisSerializer<TEntity>
    where TEntity : new()
{
    private readonly IEnumerable<PropertyInfo> _keys;
    private readonly IEnumerable<PropertyInfo> _indexes;
    private readonly IEnumerable<PropertyInfo> _properties;

    public RedisSerializer()
    {
        var allProperties = typeof(TEntity)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public);

        var clrProperties = allProperties
            .Where(x => x.PropertyType.IsSerializable);

        var sortedProperties = clrProperties
            .OrderBy(e => e.Name).ToList();

        _keys = sortedProperties
            .Where(e => e.GetCustomAttribute<KeyAttribute>() is not null);

        _properties = sortedProperties
            .Where(e => e.GetCustomAttribute<KeyAttribute>() is null);

        _indexes = sortedProperties
            .Where(e => e.GetCustomAttribute<IndexedAttribute>() is not null);
    }

    public Tuple<RedisKey, HashEntry[]> Serialize(RecordCacheKey<TEntity> key, TEntity entity)
    {
        var keyData = GetRecordKey(key, entity);

        var propertyValues = _properties.Select(e => new HashEntry(
            e.Name.ToSnakeCase(),
            JsonSerializer.Serialize(e.GetValue(entity))))
            .ToArray();

        return Tuple.Create(keyData, propertyValues);
    }

    public List<TEntity> Deserialize(RedisValue[] data)
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
            foreach (PropertyInfo key in _keys)
            {
                var keyValue = JsonSerializer.Deserialize('"' + keyRawValues[index] + '"', key.PropertyType);
                key.SetValue(entity, keyValue);

                index++;
            }

            index = 1;
            foreach (PropertyInfo property in _properties)
            {
                var propertyValue = JsonSerializer.Deserialize(materializedChunk[index].ToString(), property.PropertyType);
                property.SetValue(entity, propertyValue);

                index++;
            }
        }

        return entities;
    }

    public RedisKey GetPrimaryKey(RecordCacheKey<TEntity> key)
    {
        var keyData = new RedisKey($"{key.Name}:$$primary");
        return keyData;
    }

    public RedisValue GetRecordId(RecordCacheKey<TEntity> key, TEntity entity)
    {
        var keyValues = _keys.Select(e => JsonSerializer.Serialize(e.GetValue(entity)))
            .Select(e => e.First() == '"' && e.Last() == '"' ? e.Substring(1, e.Length - 2) : e);
        var keyData = string.Join("|", keyValues);

        return keyData;
    }

    public RedisKey GetRecordKey(RecordCacheKey<TEntity> key, TEntity entity)
    {
        var keyValues = GetRecordId(key, entity);
        var keyData = new RedisKey($"{key.Name}:{keyValues}");

        return keyData;
    }

    public RedisValue[] GetProperties(RecordCacheKey<TEntity> key)
    {
        var propertyNames = "#".Yield()
            .Concat(_properties.Select(e => $"{key.Name}:*->{e.Name.ToSnakeCase()}"))
            .Select(e => new RedisValue(e))
            .ToArray();

        return propertyNames;
    }

    public Func<IDatabase, Task> GetAddAction(RecordCacheKey<TEntity> key, TEntity entity)
    {
        var recordData = Serialize(key, entity);
        var primaryKey = GetPrimaryKey(key);
        var idKey = GetRecordId(key, entity);

        var action = async (IDatabase database) =>
        {
            var transaction = database.CreateTransaction();
            _ = transaction.HashSetAsync(recordData.Item1, recordData.Item2);
            _ = transaction.SortedSetAddAsync(primaryKey, idKey, 0);

            foreach (var index in _indexes)
            {
                var indexKey = GetIndexKey(key, index);
                var indexValue = index.GetValue(entity);
                var indexScore = GetScore(indexValue);
                _ = transaction.SortedSetAddAsync(indexKey, idKey, indexScore);
            }

            await transaction.ExecuteAsync();
        };

        return action;
    }

    public Func<IDatabase, Task> GetRemoveAction(RecordCacheKey<TEntity> key, TEntity entity)
    {
        var recordKey = GetRecordKey(key, entity);
        var primaryKey = GetPrimaryKey(key);
        var idKey = GetRecordId(key, entity);

        var action = async (IDatabase database) =>
        {
            var transaction = database.CreateTransaction();
            _ = transaction.KeyDeleteAsync(recordKey);
            _ = transaction.SortedSetRemoveAsync(primaryKey, idKey);

            foreach (var index in _indexes)
            {
                var indexKey = GetIndexKey(key, index);
                _ = transaction.SortedSetRemoveAsync(indexKey, idKey);
            }

            await transaction.ExecuteAsync();
        };

        return action;
    }

    public Func<IDatabase, Task<List<TEntity>>> GetListAction(RecordCacheKey<TEntity> key)
    {
        var primaryKey = GetPrimaryKey(key);
        var properties = GetProperties(key);

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

    private RedisKey GetIndexKey(RecordCacheKey<TEntity> key, PropertyInfo index)
    {
        var indexAttribute = index.GetCustomAttribute<IndexedAttribute>()
            ?? throw new InvalidOperationException($"Can only get an index for an indexed property, specifying an {nameof(IndexedAttribute)}.");

        var keyData = new RedisKey($"{key.Name}:${indexAttribute.Name.ToSnakeCase()}");
        return keyData;
    }

    private double GetScore(object? rawValue)
    {
        if (rawValue is null)
            return 0;

        switch (rawValue)
        {
            case bool value:
                return value.GetScore();

            case DateTime value:
                return value.GetScore();

            case DateTimeOffset value:
                return value.GetScore();

            case int value:
                return value.GetScore();

            case short value:
                return value.GetScore();

            case string value:
                return value.GetScore();

            default:
                throw new InvalidOperationException($"Unrecognized type: {rawValue?.GetType()}.");
        }
    }
}

public static class ScoreExtensions
{
    public static double GetScore(this bool @this)
    {
        return @this ? 1 : 0;
    }

    public static double GetScore(this DateTime @this)
    {
        return @this.Ticks;
    }

    public static double GetScore(this DateTimeOffset @this)
    {
        return @this.UtcTicks;
    }

    public static double GetScore(this int @this)
    {
        return @this;
    }

    public static double GetScore(this long @this)
    {
        return @this;
    }

    public static double GetScore(this short @this)
    {
        return @this;
    }

    public static double GetScore(this string @this)
    {
        var length = Math.Min(8, @this.Length);
        var preProcessed = @this.Substring(0, length).ToLower();

        var builder = new StringBuilder(preProcessed);
        for (var i = length; i < 8; i++)
        {
            builder.Append('\0');
        }

        var processed = builder.ToString();
        var bytes = Encoding.UTF8.GetBytes(processed);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        var score = BitConverter.ToDouble(bytes, 0);
        return score;
    }
}
