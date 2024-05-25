using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching;

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
        var helper = RedisHelperFactory.Instance.CreateHelper<TEntity>();
        var setKey = helper.GetEntitySetEntityKey(entity);
        KeyName = setKey.ToString();
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return (KeyName, typeof(TEntity)).GetHashCode();
    }
}
