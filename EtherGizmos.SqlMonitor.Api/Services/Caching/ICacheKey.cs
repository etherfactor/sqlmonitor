namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Represents a lockable key in a distributed cache.
/// </summary>
public interface ICacheKey
{
    /// <summary>
    /// The name of the key in the distributed cache. Should be converted to a single case.
    /// </summary>
    string KeyName { get; }
}
