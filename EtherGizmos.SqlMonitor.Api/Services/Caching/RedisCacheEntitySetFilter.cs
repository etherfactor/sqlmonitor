using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Extensions;
using StackExchange.Redis;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Applies a filter to a <see cref="RedisCacheEntitySet{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">The type of entity being cached.</typeparam>
internal class RedisCacheEntitySetFilter<TEntity> : ICacheEntitySetFilter<TEntity>
    where TEntity : new()
{
    private readonly RedisCacheEntitySet<TEntity> _cache;
    private readonly PropertyInfo _indexedProperty;
    private bool _startInclusive;
    private double _startScore;
    private bool _endInclusive;
    private double _endScore;

    /// <summary>
    /// The cached entity set being filtered.
    /// </summary>
    protected RedisCacheEntitySet<TEntity> Cache => _cache;

    public RedisCacheEntitySetFilter(RedisCacheEntitySet<TEntity> cache, PropertyInfo indexedProperty)
    {
        _cache = cache;
        _indexedProperty = indexedProperty;
    }

    /// <summary>
    /// Set the score for the filter.
    /// </summary>
    /// <param name="startInclusive">Whether to include the start value in the range.</param>
    /// <param name="startScore">The start value.</param>
    /// <param name="endInclusive">Whether to include the end value in the range.</param>
    /// <param name="endScore">The end value.</param>
    protected void SetScore(bool startInclusive, double startScore, bool endInclusive, double endScore)
    {
        _startInclusive = startInclusive;
        _startScore = startScore;
        _endInclusive = endInclusive;
        _endScore = endScore;
    }

    /// <inheritdoc/>
    public PropertyInfo GetProperty() => _indexedProperty;

    /// <inheritdoc/>
    public RedisValue GetStartScore() => new RedisValue($"{_startScore}");

    /// <inheritdoc/>
    public RedisValue GetEndScore() => new RedisValue($"{_endScore}");

    /// <inheritdoc/>
    public Exclude GetExclusivity() =>
        _startInclusive && _endInclusive ? Exclude.None
        : _startInclusive ? Exclude.Stop
        : _endInclusive ? Exclude.Start
        : Exclude.Both;
}

/// <summary>
/// Applies a filter to a <see cref="RedisCacheEntitySet{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TProperty"></typeparam>
internal class RedisCacheEntitySetFilter<TEntity, TProperty> : RedisCacheEntitySetFilter<TEntity>, ICanCompare<TEntity, TProperty>
    where TEntity : new()
{
    public RedisCacheEntitySetFilter(RedisCacheEntitySet<TEntity> cache, PropertyInfo indexedProperty) : base(cache, indexedProperty)
    {
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsBetween(TProperty valueStart, TProperty valueEnd)
    {
        SetScore(true, valueStart!.TryGetScore(), true, valueEnd!.TryGetScore());

        return new RedisCacheEntitySet<TEntity>(Cache, this);
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsEqualTo(TProperty value)
    {
        SetScore(true, value!.TryGetScore(), true, value!.TryGetScore());

        return new RedisCacheEntitySet<TEntity>(Cache, this);
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsGreaterThan(TProperty value)
    {
        SetScore(false, value!.TryGetScore(), true, double.MaxValue);

        return new RedisCacheEntitySet<TEntity>(Cache, this);
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsGreaterThanOrEqualTo(TProperty value)
    {
        SetScore(true, value!.TryGetScore(), true, double.MaxValue);

        return new RedisCacheEntitySet<TEntity>(Cache, this);
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsLessThan(TProperty value)
    {
        SetScore(true, double.MinValue, false, value!.TryGetScore());

        return new RedisCacheEntitySet<TEntity>(Cache, this);
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsLessThanOrEqualTo(TProperty value)
    {
        SetScore(true, double.MinValue, true, value!.TryGetScore());

        return new RedisCacheEntitySet<TEntity>(Cache, this);
    }
}
