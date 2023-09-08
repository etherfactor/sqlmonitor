using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Models.Extensions;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Web;

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

public interface ICacheEntity<TEntity> : ICanAlter<TEntity>, ICanGetAsync<TEntity>
{
}

public interface ICanAlter<TEntity>
{
    Task SetAsync(TEntity entity);

    Task DeleteAsync();
}

public interface ICanGetAsync<TEntity>
{
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

public static class RedisHelperCache
{
    private static readonly IDictionary<Type, object> _helpers = new Dictionary<Type, object>();

    public static RedisHelper<TEntity> For<TEntity>()
        where TEntity : new()
    {
        if (!_helpers.ContainsKey(typeof(TEntity)))
        {
            var helper = new RedisHelper<TEntity>();
            _helpers.Add(typeof(TEntity), helper);
        }

        return (RedisHelper<TEntity>)_helpers[typeof(TEntity)];
    }
}

public class RedisHelper<TEntity>
    where TEntity : new()
{
    private readonly string _tableKey;
    private readonly IEnumerable<(PropertyInfo, ColumnAttribute)> _keys;
    private readonly IEnumerable<(PropertyInfo, ColumnAttribute)> _indexes;
    private readonly IEnumerable<(PropertyInfo, ColumnAttribute)> _properties;

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
            .Where(e => e.Property.GetCustomAttribute<KeyAttribute>() is not null);

        _properties = sortedProperties
            .Where(e => e.Property.GetCustomAttribute<KeyAttribute>() is null);

        _indexes = sortedProperties
            .Where(e => e.Property.GetCustomAttribute<IndexedAttribute>() is not null);
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

    private RedisKey GetEntitySetKey(TEntity entity)
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

    private RedisKey GetEntityKey(RecordCacheKey<TEntity> key)
    {
        var keyData = new RedisKey($"{Constants.CacheSchemaName}:{key.Name}");
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

        //return input.First() == '"' && input.Last() == '"'
        //    ? input.Substring(1, input.Length - 2)
        //    : input;
    }

    private string Decode(string input)
    {
        var decoded = HttpUtility.UrlDecode(input);
        return decoded;

        //return '"' + input + '"';
    }

    public Func<IDatabase, Task> GetSetAction(RecordCacheKey<TEntity> key, TEntity entity)
    {
        var entityKey = GetEntityKey(key);
        var recordData = Serialize(entity);

        var action = async (IDatabase database) =>
        {
            await database.HashSetAsync(entityKey, recordData);
        };

        return action;
    }

    public Func<IDatabase, Task> GetDeleteAction(RecordCacheKey<TEntity> key)
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
        var setKey = GetEntitySetKey(entity);
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
        var setKey = GetEntitySetKey(entity);
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

public class CacheEntity<TEntity> : ICacheEntity<TEntity>
    where TEntity : new()
{
    private readonly RecordCacheKey<TEntity> _key;
    private readonly IDatabase _database;

    public CacheEntity(RecordCacheKey<TEntity> key, IDatabase database)
    {
        _key = key;
        _database = database;
    }

    public async Task DeleteAsync()
    {
        var serializer = RedisHelperCache.For<TEntity>();
        var action = serializer.GetDeleteAction(_key);
        await action(_database);
    }

    public async Task SetAsync(TEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var serializer = RedisHelperCache.For<TEntity>();
        var action = serializer.GetSetAction(_key, entity);
        await action(_database);
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
        var action = serializer.GetAddAction(entity);
        await action(_database);
    }

    public async Task RemoveAsync(TEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var serializer = RedisHelperCache.For<TEntity>();
        var action = serializer.GetRemoveAction(entity);
        await action(_database);
    }

    public async Task<List<TEntity>> ToListAsync()
    {
        var serializer = RedisHelperCache.For<TEntity>();
        var action = serializer.GetListAction(_filters);
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
