using EtherGizmos.SqlMonitor.Shared.Utilities.Extensions;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;

/// <summary>
/// A lockable job in a distributed cache.
/// </summary>
public readonly struct JobCacheKey : ICacheKey
{
    /// <inheritdoc/>
    public readonly string Name { get; }

    /// <summary>
    /// Use <see cref="CacheKey.ForJob(string)"/> instead!
    /// </summary>
    public JobCacheKey(Type jobType)
    {
        Name = jobType.Name.ToSnakeCase();
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
