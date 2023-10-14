using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Extensions;
using System.Linq.Expressions;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

internal class CacheEntitySetFiltered<TEntity> : ICanFilter<TEntity>, ICacheFiltered<TEntity>
    where TEntity : new()
{
    private readonly ICacheEntitySet<TEntity> _cache;
    private readonly IEnumerable<ICacheEntitySetFilter<TEntity>> _filters;

    public CacheEntitySetFiltered(ICacheEntitySet<TEntity> cache, IEnumerable<ICacheEntitySetFilter<TEntity>> filters)
    {
        _cache = cache;
        _filters = filters;
    }

    public async Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
    {
        return await _cache.ToListAsync(_filters);
    }

    public ICanCompare<TEntity, TProperty> Where<TProperty>(Expression<Func<TEntity, TProperty>> indexedProperty)
    {
        var propertyInfo = indexedProperty.GetPropertyInfo();
        if (propertyInfo.GetCustomAttribute<IndexedAttribute>() is null)
            throw new InvalidOperationException("Can only filter on an indexed property.");

        return new CacheEntitySetFilter<TEntity, TProperty>(_cache, _filters, propertyInfo);
    }

    Task<List<TEntity>> ICanList<TEntity>.ToListAsync(IEnumerable<ICacheEntitySetFilter<TEntity>> filters, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
