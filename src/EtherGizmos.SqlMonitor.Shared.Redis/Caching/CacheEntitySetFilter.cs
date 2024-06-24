using EtherGizmos.SqlMonitor.Shared.Redis.Annotations;
using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Redis.Extensions;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching;

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
        var minScore = GetScore(valueStart);
        var maxScore = GetScore(valueEnd);
        SetScore(true, minScore, true, maxScore);

        return new CacheEntitySetFiltered<TEntity>(Cache, CurrentFilters.Append(this));
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsEqualTo(TProperty value)
    {
        var score = GetScore(value);
        SetScore(true, score, true, score);

        return new CacheEntitySetFiltered<TEntity>(Cache, CurrentFilters.Append(this));
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsGreaterThan(TProperty value)
    {
        var score = GetScore(value);
        SetScore(false, score, true, double.MaxValue);

        return new CacheEntitySetFiltered<TEntity>(Cache, CurrentFilters.Append(this));
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsGreaterThanOrEqualTo(TProperty value)
    {
        var score = GetScore(value);
        SetScore(true, score, true, double.MaxValue);

        return new CacheEntitySetFiltered<TEntity>(Cache, CurrentFilters.Append(this));
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsLessThan(TProperty value)
    {
        var score = GetScore(value);
        SetScore(true, double.MinValue, false, score);

        return new CacheEntitySetFiltered<TEntity>(Cache, CurrentFilters.Append(this));
    }

    /// <inheritdoc/>
    public ICacheFiltered<TEntity> IsLessThanOrEqualTo(TProperty value)
    {
        var score = GetScore(value);
        SetScore(true, double.MinValue, true, score);

        return new CacheEntitySetFiltered<TEntity>(Cache, CurrentFilters.Append(this));
    }

    private double GetScore(TProperty value)
    {
        double score;
        if (value?.GetType()?.IsAssignableTo(typeof(string)) == true
            && GetProperty().GetCustomAttribute<CaseSensitiveAttribute>() is null)
        {
            score = (value as string)?.ToUpper()?.TryGetScore() ?? 0;
        }
        else
        {
            score = value?.TryGetScore() ?? 0;
        }

        return score;
    }
}
