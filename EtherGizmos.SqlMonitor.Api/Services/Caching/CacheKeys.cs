namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Pre-constructed cache keys for easy access.
/// </summary>
public static class CacheKeys
{
    //Job Keys
    public static JobCacheKey EnqueueMonitorQueries { get; } = CacheKey.ForJob("EnqueueMonitorQueries");

    //Entity Keys
}
