using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Models.Extensions;
using StackExchange.Redis;
using System.Linq.Expressions;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public class CacheEntitySet<TEntity> : ICacheEntitySet<TEntity>
    where TEntity : new()
{
    private readonly IDatabase _database;
    private readonly IEnumerable<CacheEntitySetFilter<TEntity>> _filters;

    public CacheEntitySet(IDatabase database)
    {
        _database = database;
        _filters = Enumerable.Empty<CacheEntitySetFilter<TEntity>>();
    }

    internal CacheEntitySet(CacheEntitySet<TEntity> cache, CacheEntitySetFilter<TEntity> newFilter)
    {
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
