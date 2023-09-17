using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Pre-constructed cache keys for easy access.
/// </summary>
public static class CacheKeys
{
    //Job Keys
    public static JobCacheKey EnqueueMonitorQueries { get; } = CacheKey.ForJob("EnqueueMonitorQueries");

    //Entity Keys
    public static EntityCacheKey<List<Query>> AllQueries { get; } = CacheKey.ForEntity<List<Query>>("AllQueries");

    public static EntityCacheKey<List<Instance>> AllInstances { get; } = CacheKey.ForEntity<List<Instance>>("AllInstances");
}
