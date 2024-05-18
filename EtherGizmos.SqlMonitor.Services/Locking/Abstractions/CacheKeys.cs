namespace EtherGizmos.SqlMonitor.Services.Locking.Abstractions;

/// <summary>
/// Pre-constructed cache keys for easy access.
/// </summary>
public static class CacheKeys
{
    //Job Keys
    public static JobCacheKey EnqueueMonitorQueries { get; } = CacheKey.ForJob("EnqueueMonitorQueries");

    //Entity Keys
}
