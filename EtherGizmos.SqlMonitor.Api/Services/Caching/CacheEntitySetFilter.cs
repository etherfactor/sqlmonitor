using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Applies a filter to a <see cref="RedisCacheEntitySet{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">The type of entity being cached.</typeparam>
internal class CacheEntitySetFilter<TEntity> : ICacheEntitySetFilter<TEntity>
    where TEntity : new()
{
    private readonly ICacheEntitySet<TEntity> _cache;
    private readonly IEnumerable<ICacheEntitySetFilter<TEntity>> _currentFilters;
    private readonly PropertyInfo _indexedProperty;
    private bool _startInclusive;
    private double _startScore;
    private bool _endInclusive;
    private double _endScore;

    /// <summary>
    /// The cached entity set being filtered.
    /// </summary>
    protected ICacheEntitySet<TEntity> Cache => _cache;

    /// <summary>
    /// The currently applied entity set filters.
    /// </summary>
    protected IEnumerable<ICacheEntitySetFilter<TEntity>> CurrentFilters => _currentFilters;

    public CacheEntitySetFilter(ICacheEntitySet<TEntity> cache, IEnumerable<ICacheEntitySetFilter<TEntity>> currentFilters, PropertyInfo indexedProperty)
    {
        _cache = cache;
        _currentFilters = currentFilters;
        _indexedProperty = indexedProperty;
    }

    /// <inheritdoc/>
    public bool GetEndInclusivity() => _endInclusive;

    /// <inheritdoc/>
    public double GetEndScore() => _endScore;

    /// <inheritdoc/>
    public PropertyInfo GetProperty() => _indexedProperty;

    /// <inheritdoc/>
    public bool GetStartInclusivity() => _startInclusive;

    /// <inheritdoc/>
    public double GetStartScore() => _startScore;

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
}

/// <summary>
/// Applies a filter to a <see cref="RedisCacheEntitySet{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TProperty"></typeparam>
internal class CacheEntitySetFilter<TEntity, TProperty> : CacheEntitySetFilter<TEntity>, ICanCompare<TEntity, TProperty>
    where TEntity : new()
{
    public CacheEntitySetFilter(ICacheEntitySet<TEntity> cache, IEnumerable<ICacheEntitySetFilter<TEntity>> currentFilters, PropertyInfo indexedProperty) : base(cache, currentFilters, indexedProperty)
    {
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsBetween(TProperty valueStart, TProperty valueEnd)
    {
        SetScore(true, valueStart!.TryGetScore(), true, valueEnd!.TryGetScore());

        return new CacheEntitySetFiltered<TEntity>(Cache, CurrentFilters.Append(this));
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsEqualTo(TProperty value)
    {
        SetScore(true, value!.TryGetScore(), true, value!.TryGetScore());

        return new CacheEntitySetFiltered<TEntity>(Cache, CurrentFilters.Append(this));
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsGreaterThan(TProperty value)
    {
        SetScore(false, value!.TryGetScore(), true, double.MaxValue);

        return new CacheEntitySetFiltered<TEntity>(Cache, CurrentFilters.Append(this));
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsGreaterThanOrEqualTo(TProperty value)
    {
        SetScore(true, value!.TryGetScore(), true, double.MaxValue);

        return new CacheEntitySetFiltered<TEntity>(Cache, CurrentFilters.Append(this));
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsLessThan(TProperty value)
    {
        SetScore(true, double.MinValue, false, value!.TryGetScore());

        return new CacheEntitySetFiltered<TEntity>(Cache, CurrentFilters.Append(this));
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsLessThanOrEqualTo(TProperty value)
    {
        SetScore(true, double.MinValue, true, value!.TryGetScore());

        return new CacheEntitySetFiltered<TEntity>(Cache, CurrentFilters.Append(this));
    }
}
