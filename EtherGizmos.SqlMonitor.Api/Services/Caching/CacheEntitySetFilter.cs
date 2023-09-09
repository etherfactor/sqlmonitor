﻿using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Extensions;
using StackExchange.Redis;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Applies a filter to a <see cref="CacheEntitySet{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">The type of entity being cached.</typeparam>
internal class CacheEntitySetFilter<TEntity>
    where TEntity : new()
{
    private readonly CacheEntitySet<TEntity> _cache;
    private readonly PropertyInfo _indexedProperty;
    private bool _startInclusive;
    private double _startScore;
    private bool _endInclusive;
    private double _endScore;

    /// <summary>
    /// The cached entity set being filtered.
    /// </summary>
    protected CacheEntitySet<TEntity> Cache => _cache;

    public CacheEntitySetFilter(CacheEntitySet<TEntity> cache, PropertyInfo indexedProperty)
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

    /// <summary>
    /// Get the indexed property of the filter.
    /// </summary>
    /// <returns>The indexed property.</returns>
    public PropertyInfo GetProperty() => _indexedProperty;

    /// <summary>
    /// Get the start score of the filter.
    /// </summary>
    /// <returns>The start score.</returns>
    public RedisValue GetStartScore() => new RedisValue($"{_startScore}");
    
    /// <summary>
    /// Get the end score of the filter.
    /// </summary>
    /// <returns>The end score.</returns>
    public RedisValue GetEndScore() => new RedisValue($"{_endScore}");

    /// <summary>
    /// Gets which of the filter's scores are inclusive vs. exclusive.
    /// </summary>
    /// <returns></returns>
    public Exclude GetExclusivity() =>
        _startInclusive && _endInclusive ? Exclude.None
        : _startInclusive ? Exclude.Stop
        : _endInclusive ? Exclude.Start
        : Exclude.Both;
}

/// <summary>
/// Applies a filter to a <see cref="CacheEntitySet{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TProperty"></typeparam>
internal class CacheEntitySetFilter<TEntity, TProperty> : CacheEntitySetFilter<TEntity>, ICanCompare<TEntity, TProperty>
    where TEntity : new()
{
    public CacheEntitySetFilter(CacheEntitySet<TEntity> cache, PropertyInfo indexedProperty) : base(cache, indexedProperty)
    {
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsBetween(TProperty valueStart, TProperty valueEnd)
    {
        SetScore(true, valueStart!.TryGetScore(), true, valueEnd!.TryGetScore());

        return new CacheEntitySet<TEntity>(Cache, this);
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsEqualTo(TProperty value)
    {
        SetScore(true, value!.TryGetScore(), true, value!.TryGetScore());

        return new CacheEntitySet<TEntity>(Cache, this);
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsGreaterThan(TProperty value)
    {
        SetScore(false, value!.TryGetScore(), true, double.MaxValue);

        return new CacheEntitySet<TEntity>(Cache, this);
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsGreaterThanOrEqualTo(TProperty value)
    {
        SetScore(true, value!.TryGetScore(), true, double.MaxValue);

        return new CacheEntitySet<TEntity>(Cache, this);
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsLessThan(TProperty value)
    {
        SetScore(true, double.MinValue, false, value!.TryGetScore());

        return new CacheEntitySet<TEntity>(Cache, this);
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsLessThanOrEqualTo(TProperty value)
    {
        SetScore(true, double.MinValue, true, value!.TryGetScore());

        return new CacheEntitySet<TEntity>(Cache, this);
    }
}
