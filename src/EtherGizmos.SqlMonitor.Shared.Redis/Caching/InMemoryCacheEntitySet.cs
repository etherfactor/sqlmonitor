﻿using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Redis.Extensions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Annotations;
using EtherGizmos.SqlMonitor.Shared.Utilities.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching;

/// <summary>
/// Provides means for caching and retrieving entities in a set, in memory.
/// </summary>
/// <typeparam name="TEntity">The type of entity being cached.</typeparam>
internal class InMemoryCacheEntitySet<TEntity> : ICacheEntitySet<TEntity>
    where TEntity : class, new()
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly IDictionary<string, TEntity> _entities = new Dictionary<string, TEntity>();

    public InMemoryCacheEntitySet(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var factory = _serviceProvider.GetRequiredService<IRedisHelperFactory>();

        var helper = factory.CreateHelper<TEntity>();
        var id = helper.GetEntitySetEntityKey(entity).ToString();

        //Forcefully detach the entity from any contexts
        var jsonOptions = new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve };
        var addEntity = JsonSerializer.Deserialize<TEntity>(JsonSerializer.Serialize(entity, jsonOptions), jsonOptions)!;
        if (!_entities.ContainsKey(id))
        {
            _entities.Add(id, addEntity);
        }

        _entities[id] = addEntity;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<TEntity?> GetAsync(object[] keys, CancellationToken cancellationToken = default)
    {
        if (keys.Length == 0)
            throw new ArgumentException("Must provide at least one key.", nameof(keys));

        var factory = _serviceProvider.GetRequiredService<IRedisHelperFactory>();

        var helper = factory.CreateHelper<TEntity>();
        var entitySetKey = helper.GetEntitySetEntityKey(keys).ToString();

        if (_entities.ContainsKey(entitySetKey))
        {
            return Task.FromResult<TEntity?>(_entities[entitySetKey]);
        }

        return Task.FromResult<TEntity?>(null);
    }

    /// <inheritdoc/>
    public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var factory = _serviceProvider.GetRequiredService<IRedisHelperFactory>();

        var helper = factory.CreateHelper<TEntity>();
        var id = helper.GetEntitySetEntityKey(entity).ToString();
        if (_entities.ContainsKey(id))
        {
            _entities.Remove(id);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
    {
        var entities = _entities.Values.ToList();
        return Task.FromResult(entities);
    }

    /// <inheritdoc/>
    public ICanCompare<TEntity, TProperty> Where<TProperty>(Expression<Func<TEntity, TProperty>> indexedProperty)
    {
        var propertyInfo = indexedProperty.GetPropertyInfo();
        if (propertyInfo.GetCustomAttribute<IndexedAttribute>() is null)
            throw new InvalidOperationException("Can only filter on an indexed property.");

        return new CacheEntitySetFilter<TEntity, TProperty>(this, Enumerable.Empty<ICacheEntitySetFilter<TEntity>>(), propertyInfo);
    }

    Task<List<TEntity>> ICanList<TEntity>.ToListAsync(IEnumerable<ICacheEntitySetFilter<TEntity>> filters, CancellationToken cancellationToken)
    {
        var entities = _entities.Values
            .Where(e =>
            {
                var overall = true;
                foreach (var filter in filters)
                {
                    var entityScore = filter.GetProperty()
                        .GetValue(e)
                        ?.TryGetScore() ?? 0;

                    var startScore = double.Parse(filter.GetStartScore().ToString());
                    var endScore = double.Parse(filter.GetEndScore().ToString());

                    bool left;
                    if (filter.GetStartInclusivity())
                    {
                        left = entityScore >= startScore;
                    }
                    else
                    {
                        left = entityScore > startScore;
                    }

                    bool right;
                    if (filter.GetEndInclusivity())
                    {
                        right = entityScore <= endScore;
                    }
                    else
                    {
                        right = entityScore < endScore;
                    }

                    overall &= left && right;
                }

                return overall;
            })
            .ToList();
        return Task.FromResult(entities);
    }
}
