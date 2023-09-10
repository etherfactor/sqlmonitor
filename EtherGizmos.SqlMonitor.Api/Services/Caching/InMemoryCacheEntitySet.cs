using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Extensions;
using EtherGizmos.SqlMonitor.Models.Extensions;
using StackExchange.Redis;
using System.Linq.Expressions;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Provides means for caching and retrieving entities in a set, in memory.
/// </summary>
/// <typeparam name="TEntity">The type of entity being cached.</typeparam>
internal class InMemoryCacheEntitySet<TEntity> : ICacheEntitySet<TEntity>
    where TEntity : new()
{
    private static readonly IDictionary<string, TEntity> _entities = new Dictionary<string, TEntity>();
    private readonly IEnumerable<InMemoryCacheEntitySetFilter<TEntity>> _filters;

    public InMemoryCacheEntitySet()
    {
        _filters = Enumerable.Empty<InMemoryCacheEntitySetFilter<TEntity>>();
    }

    internal InMemoryCacheEntitySet(InMemoryCacheEntitySet<TEntity> cache, InMemoryCacheEntitySetFilter<TEntity> newFilter)
    {
        _filters = cache._filters.Append(newFilter);
    }

    /// <inheritdoc/>
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var helper = RedisHelperCache.For<TEntity>();
        var id = helper.GetSetEntityKey(entity).ToString();
        if (!_entities.ContainsKey(id))
        {
            _entities.Add(id, entity);
        }

        _entities[id] = entity;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var helper = RedisHelperCache.For<TEntity>();
        var id = helper.GetSetEntityKey(entity).ToString();
        if (_entities.ContainsKey(id))
        {
            _entities.Remove(id);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
    {
        var entities = _entities.Values
            .Where(e =>
            {
                var overall = true;
                foreach (var filter in _filters)
                {
                    var entityScore = filter.GetProperty()
                        .GetValue(e)
                        ?.TryGetScore() ?? 0;

                    var startScore = double.Parse(filter.GetStartScore().ToString());
                    var endScore = double.Parse(filter.GetEndScore().ToString());

                    bool left;
                    if (filter.GetExclusivity() == Exclude.Start || filter.GetExclusivity() == Exclude.Both)
                    {
                        left = entityScore > startScore;
                    }
                    else
                    {
                        left = entityScore >= startScore;
                    }

                    bool right;
                    if (filter.GetExclusivity() == Exclude.Stop || filter.GetExclusivity() == Exclude.Both)
                    {
                        right = entityScore < endScore;
                    }
                    else
                    {
                        right = entityScore <= endScore;
                    }

                    overall &= left && right;
                }

                return overall;
            })
            .ToList();
        return Task.FromResult(entities);
    }

    /// <inheritdoc/>
    public ICanCompare<TEntity, TProperty> Where<TProperty>(Expression<Func<TEntity, TProperty>> indexedProperty)
    {
        var propertyInfo = indexedProperty.GetPropertyInfo();
        if (propertyInfo.GetCustomAttribute<IndexedAttribute>() is null)
            throw new InvalidOperationException("Can only filter on an indexed property.");

        return new InMemoryCacheEntitySetFilter<TEntity, TProperty>(this, propertyInfo);
    }
}
