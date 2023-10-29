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
    where TEntity : class, new()
{
    private readonly IRedisHelperFactory _factory;
    private readonly IDatabase _database;

    public RedisCacheEntitySet(IRedisHelperFactory factory, IDatabase database)
    {
        _factory = factory;
        _database = database;
    }

    /// <inheritdoc/>
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var serializer = _factory.CreateHelper<TEntity>();

        var transaction = _database.CreateTransaction();
        serializer.AppendAddAction(_database, transaction, entity);

        await transaction.ExecuteAsync();
    }

    /// <inheritdoc/>
    public async Task<TEntity?> GetAsync(object[] keys, CancellationToken cancellationToken = default)
    {
        if (keys.Length == 0)
            throw new ArgumentException("Must provide at least one key.", nameof(keys));

        var serializer = _factory.CreateHelper<TEntity>();

        var entitySetKey = serializer.GetEntitySetEntityKey(keys);

        var transaction = _database.CreateTransaction();
        var action = serializer.AppendReadAction(_database, transaction, entitySetKey);

        await transaction.ExecuteAsync();
        return await action();
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var serializer = _factory.CreateHelper<TEntity>();

        var transaction = _database.CreateTransaction();
        serializer.AppendRemoveAction(_database, transaction, entity);

        await transaction.ExecuteAsync();
    }

    /// <inheritdoc/>
    public async Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
    {
        var serializer = _factory.CreateHelper<TEntity>();

        var transaction = _database.CreateTransaction();
        var action = serializer.AppendListAction(_database, transaction, filters: Enumerable.Empty<ICacheEntitySetFilter<TEntity>>());

        await transaction.ExecuteAsync();
        return await action();
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
        var serializer = _factory.CreateHelper<TEntity>();

        var transaction = _database.CreateTransaction();
        var action = serializer.AppendListAction(_database, transaction, filters);

        await transaction.ExecuteAsync();
        return await action();
    }
}
