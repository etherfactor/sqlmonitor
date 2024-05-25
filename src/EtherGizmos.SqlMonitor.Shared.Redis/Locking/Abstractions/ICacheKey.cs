namespace EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;

/// <summary>
/// Represents a lockable key in a distributed cache.
/// </summary>
public interface ICacheKey
{
    /// <summary>
    /// The name of the key in the distributed cache. Implementations should return this value in a single case.
    /// </summary>
    string KeyName { get; }
}
