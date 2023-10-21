using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// A lockable entity contained in a set in a distributed cache.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public struct EntitySetRecordCacheKey<TEntity> : ICacheKey
    where TEntity : class, new()
{
    /// <inheritdoc/>
    public readonly string KeyName { get; }

    /// <summary>
    /// Use <see cref="CacheKey.ForEntitySetRecord{TEntity}(TEntity)"/> instead!
    /// </summary>
    internal EntitySetRecordCacheKey(TEntity entity)
    {
        var helper = RedisHelperCache.For<TEntity>();
        var setKey = helper.GetSetEntityKey(entity);
        KeyName = setKey.ToString();
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return (KeyName, typeof(TEntity)).GetHashCode();
    }
}
