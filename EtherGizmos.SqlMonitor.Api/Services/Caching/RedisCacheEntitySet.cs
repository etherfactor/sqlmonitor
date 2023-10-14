using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Extensions;
using StackExchange.Redis;
using System.Linq.Expressions;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Provides means for caching and retrieving entities in a set, in Redis.
/// </summary>
/// <typeparam name="TEntity">The type of entity being cached.</typeparam>
internal class RedisCacheEntitySet<TEntity> : ICacheEntitySet<TEntity>
    where TEntity : new()
{
    private readonly IDatabase _database;

    public RedisCacheEntitySet(IDatabase database)
    {
        _database = database;
    }

    /// <inheritdoc/>
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var serializer = RedisHelperCache.For<TEntity>();
        var action = serializer.GetAddAction(entity);
        await action(_database);
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var serializer = RedisHelperCache.For<TEntity>();
        var action = serializer.GetRemoveAction(entity);
        await action(_database);
    }

    /// <inheritdoc/>
    public async Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
    {
        var serializer = RedisHelperCache.For<TEntity>();
        var action = serializer.GetListAction(Enumerable.Empty<ICacheEntitySetFilter<TEntity>>());
        return await action(_database);
    }

    /// <inheritdoc/>
    public ICanCompare<TEntity, TProperty> Where<TProperty>(Expression<Func<TEntity, TProperty>> indexedProperty)
    {
        var propertyInfo = indexedProperty.GetPropertyInfo();
        if (propertyInfo.GetCustomAttribute<IndexedAttribute>() is null)
            throw new InvalidOperationException("Can only filter on an indexed property.");

        return new CacheEntitySetFilter<TEntity, TProperty>(this, Enumerable.Empty<ICacheEntitySetFilter<TEntity>>(), propertyInfo);
    }

    /// <inheritdoc/>
    async Task<List<TEntity>> ICanList<TEntity>.ToListAsync(IEnumerable<ICacheEntitySetFilter<TEntity>> filters, CancellationToken cancellationToken)
    {
        var serializer = RedisHelperCache.For<TEntity>();
        var action = serializer.GetListAction(filters);
        return await action(_database);
    }
}
