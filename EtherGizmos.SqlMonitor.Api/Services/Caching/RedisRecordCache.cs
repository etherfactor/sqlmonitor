using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Models.Extensions;
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

public interface ICacheEntitySet<TEntity> : ICanAlterSet<TEntity>, ICacheFiltered<TEntity>
{
}

public interface ICacheFiltered<TEntity> : ICanFilter<TEntity>, ICanList<TEntity>
{
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
    ICacheFiltered<TEntity> IsEqualTo(TProperty value);

    ICacheFiltered<TEntity> IsGreaterThan(TProperty value);

    ICacheFiltered<TEntity> IsGreaterThanOrEqualTo(TProperty value);

    ICacheFiltered<TEntity> IsLessThan(TProperty value);

    ICacheFiltered<TEntity> IsLessThanOrEqualTo(TProperty value);

    ICacheFiltered<TEntity> IsBetween(TProperty valueStart, TProperty valueEnd);
}

public static class ICanCompareExtensions
{
    public static ICanFilter<TEntity> StartsWith<TEntity>(this ICanCompare<TEntity, string> @this)
    {
        throw new NotImplementedException();
    }
}

public static class RedisHelperCache
{
    private static readonly IDictionary<Type, object> _serializers = new Dictionary<Type, object>();

    public static RedisHelper<TEntity> For<TEntity>()
        where TEntity : new()
    {
        if (!_serializers.ContainsKey(typeof(TEntity)))
        {
            var serializer = new RedisHelper<TEntity>();
            _serializers.Add(typeof(TEntity), serializer);
        }

        return (RedisHelper<TEntity>)_serializers[typeof(TEntity)];
    }
}

public class RedisHelper<TEntity>
    where TEntity : new()
{
    private readonly IEnumerable<PropertyInfo> _keys;
    private readonly IEnumerable<PropertyInfo> _indexes;
    private readonly IEnumerable<PropertyInfo> _properties;

    public RedisHelper()
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

    private Tuple<RedisKey, HashEntry[]> Serialize(RecordCacheKey<TEntity> key, TEntity entity)
    {
        var keyData = GetRecordKey(key, entity);

        var propertyValues = _properties.Select(e => new HashEntry(
            e.Name.ToSnakeCase(),
            JsonSerializer.Serialize(e.GetValue(entity))))
            .ToArray();

        return Tuple.Create(keyData, propertyValues);
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

    private RedisKey GetPrimaryKey(RecordCacheKey<TEntity> key)
    {
        var keyData = new RedisKey($"{key.Name}:$$primary");
        return keyData;
    }

    private RedisValue GetRecordId(RecordCacheKey<TEntity> key, TEntity entity)
    {
        var keyValues = _keys.Select(e => JsonSerializer.Serialize(e.GetValue(entity)))
            .Select(e => e.First() == '"' && e.Last() == '"' ? e.Substring(1, e.Length - 2) : e);
        var keyData = string.Join("|", keyValues);

        return keyData;
    }

    private RedisKey GetRecordKey(RecordCacheKey<TEntity> key, TEntity entity)
    {
        var keyValues = GetRecordId(key, entity);
        var keyData = new RedisKey($"{key.Name}:{keyValues}");

        return keyData;
    }

    private RedisValue[] GetProperties(RecordCacheKey<TEntity> key)
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
                var indexScore = indexValue!.TryGetScore();
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

    public Func<IDatabase, Task<List<TEntity>>> GetListAction(RecordCacheKey<TEntity> key, IEnumerable<CacheEntitySetFilter<TEntity>> filters)
    {
        var primaryKey = GetPrimaryKey(key);
        var properties = GetProperties(key);

        if (filters.Any())
        {
            var action = async (IDatabase database) =>
            {
                var transaction = database.CreateTransaction();

                var allTempKeys = new List<RedisKey>();
                foreach (var filter in filters)
                {
                    var indexProperty = filter.GetProperty();
                    var indexKey = GetIndexKey(key, indexProperty);

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

    private RedisKey GetIndexKey(RecordCacheKey<TEntity> key, PropertyInfo index)
    {
        var indexAttribute = index.GetCustomAttribute<IndexedAttribute>()
            ?? throw new InvalidOperationException($"Can only get an index for an indexed property, specifying an {nameof(IndexedAttribute)}.");

        var keyData = new RedisKey($"{key.Name}:${indexAttribute.Name.ToSnakeCase()}");
        return keyData;
    }

    private RedisKey GetTempKey()
    {
        var guid = Guid.NewGuid();
        var tempKey = new RedisKey($"sqlpulse:$$temp:{guid}");
        return tempKey;
    }
}

public class CacheEntitySet<TEntity> : ICacheEntitySet<TEntity>
    where TEntity : new()
{
    private readonly RecordCacheKey<TEntity> _key;
    private readonly IDatabase _database;
    private readonly IEnumerable<CacheEntitySetFilter<TEntity>> _filters;

    public CacheEntitySet(RecordCacheKey<TEntity> key, IDatabase database)
    {
        _key = key;
        _database = database;
        _filters = Enumerable.Empty<CacheEntitySetFilter<TEntity>>();
    }

    internal CacheEntitySet(CacheEntitySet<TEntity> cache, CacheEntitySetFilter<TEntity> newFilter)
    {
        _key = cache._key;
        _database = cache._database;
        _filters = cache._filters.Append(newFilter);
    }

    public async Task AddAsync(TEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var serializer = RedisHelperCache.For<TEntity>();
        var action = serializer.GetAddAction(_key, entity);
        await action(_database);
    }

    public async Task RemoveAsync(TEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var serializer = RedisHelperCache.For<TEntity>();
        var action = serializer.GetRemoveAction(_key, entity);
        await action(_database);
    }

    public async Task<List<TEntity>> ToListAsync()
    {
        var serializer = RedisHelperCache.For<TEntity>();
        var action = serializer.GetListAction(_key, _filters);
        return await action(_database);
    }

    public ICanCompare<TEntity, TProperty> Where<TProperty>(Expression<Func<TEntity, TProperty>> indexedProperty)
    {
        var propertyInfo = indexedProperty.GetPropertyInfo();
        if (propertyInfo.GetCustomAttribute<IndexedAttribute>() is null)
            throw new InvalidOperationException("Can only filter on an indexed property.");

        return new CacheEntitySetFilter<TEntity, TProperty>(this, propertyInfo);
    }

    public IEnumerable<CacheEntitySetFilter<TEntity>> GetFilters() => _filters;
}

public class CacheEntitySetFilter<TEntity>
    where TEntity : new()
{
    private readonly CacheEntitySet<TEntity> _cache;
    private readonly PropertyInfo _indexedProperty;
    private bool _startInclusive;
    private double _startScore;
    private bool _endInclusive;
    private double _endScore;

    protected CacheEntitySet<TEntity> Cache => _cache;

    public CacheEntitySetFilter(CacheEntitySet<TEntity> cache, PropertyInfo indexedProperty)
    {
        _cache = cache;
        _indexedProperty = indexedProperty;
    }

    protected void SetScore(bool startInclusive, double startScore, bool endInclusive, double endScore)
    {
        _startInclusive = startInclusive;
        _startScore = startScore;
        _endInclusive = endInclusive;
        _endScore = endScore;
    }

    public PropertyInfo GetProperty() => _indexedProperty;

    public RedisValue GetStartScore() => new RedisValue($"{_startScore}");

    public RedisValue GetEndScore() => new RedisValue($"{_endScore}");

    public Exclude GetExclusivity() =>
        _startInclusive && _endInclusive ? Exclude.None
        : _startInclusive ? Exclude.Stop
        : _endInclusive ? Exclude.Start
        : Exclude.Both;
}

public class CacheEntitySetFilter<TEntity, TProperty> : CacheEntitySetFilter<TEntity>, ICanCompare<TEntity, TProperty>
    where TEntity : new()
{
    public CacheEntitySetFilter(CacheEntitySet<TEntity> cache, PropertyInfo indexedProperty) : base(cache, indexedProperty)
    {
    }

    public ICacheFiltered<TEntity> IsBetween(TProperty valueStart, TProperty valueEnd)
    {
        SetScore(true, valueStart!.TryGetScore(), true, valueEnd!.TryGetScore());

        return new CacheEntitySet<TEntity>(Cache, this);
    }

    public ICacheFiltered<TEntity> IsEqualTo(TProperty value)
    {
        SetScore(true, value!.TryGetScore(), true, value!.TryGetScore());

        return new CacheEntitySet<TEntity>(Cache, this);
    }

    public ICacheFiltered<TEntity> IsGreaterThan(TProperty value)
    {
        SetScore(false, value!.TryGetScore(), true, double.MaxValue);

        return new CacheEntitySet<TEntity>(Cache, this);
    }

    public ICacheFiltered<TEntity> IsGreaterThanOrEqualTo(TProperty value)
    {
        SetScore(true, value!.TryGetScore(), true, double.MaxValue);

        return new CacheEntitySet<TEntity>(Cache, this);
    }

    public ICacheFiltered<TEntity> IsLessThan(TProperty value)
    {
        SetScore(true, double.MinValue, false, value!.TryGetScore());

        return new CacheEntitySet<TEntity>(Cache, this);
    }

    public ICacheFiltered<TEntity> IsLessThanOrEqualTo(TProperty value)
    {
        SetScore(true, double.MinValue, true, value!.TryGetScore());

        return new CacheEntitySet<TEntity>(Cache, this);
    }
}

public static class ScoreExtensions
{
    public static double TryGetScore(this object @this)
    {
        if (@this is null)
            return 0;

        switch (@this)
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
                throw new InvalidOperationException($"Unrecognized type: {@this.GetType()}.");
        }
    }

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
