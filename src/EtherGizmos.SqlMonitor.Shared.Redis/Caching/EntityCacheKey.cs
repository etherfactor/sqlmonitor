﻿using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Extensions;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching;

/// <summary>
/// A lockable entity in a distributed cache.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public readonly struct EntityCacheKey<TEntity> : ICacheKey
{
    /// <inheritdoc/>
    public readonly string Name { get; }

    /// <summary>
    /// Use <see cref="CacheKey.ForEntity{TEntity}(string)"/> instead!
    /// </summary>
    internal EntityCacheKey(string keyName)
    {
        Name = keyName.ToSnakeCase();
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return (Name, typeof(TEntity)).GetHashCode();
    }
}
