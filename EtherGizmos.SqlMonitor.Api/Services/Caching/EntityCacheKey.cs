using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// A lockable entity in a distributed cache.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public readonly struct EntityCacheKey<TEntity> : ICacheKey
{
    /// <inheritdoc/>
    public readonly string KeyName { get; }

    /// <summary>
    /// Use <see cref="CacheKey.ForEntity{TEntity}(string)"/> instead!
    /// </summary>
    internal EntityCacheKey(string keyName)
    {
        KeyName = keyName.ToSnakeCase();
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return (KeyName, typeof(TEntity)).GetHashCode();
    }
}
