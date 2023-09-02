using EtherGizmos.SqlMonitor.Api.Extensions;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// A lockable entity in a distributed cache.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public readonly struct EntityCacheKey<TEntity> : ICacheKey
{
    /// <summary>
    /// The name of the entity.
    /// </summary>
    public readonly string Name { get; }

    /// <inheritdoc/>
    public readonly string KeyName => $"{Constants.CacheSchemaName}:$entity:{Name.ToSnakeCase()}";

    /// <summary>
    /// Indicates if a lock is required to edit the key.
    /// </summary>
    public readonly bool RequiresLock { get; }

    /// <summary>
    /// Use <see cref="CacheKey.CreateEntity{TEntity}(string, bool)"/> instead!
    /// </summary>
    internal EntityCacheKey(string name, bool requiresLock)
    {
        Name = name;
        RequiresLock = requiresLock;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return (Name, typeof(TEntity), RequiresLock).GetHashCode();
    }
}
