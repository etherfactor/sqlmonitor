using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Extensions;
using StackExchange.Redis;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

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
