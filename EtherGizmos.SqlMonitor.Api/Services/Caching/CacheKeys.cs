using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Pre-constructed cache keys for easy access.
/// </summary>
public static class CacheKeys
{
    //Job Keys
    public static JobCacheKey EnqueueMonitorQueries { get; } = CacheKey.CreateJob("EnqueueMonitorQueries");

    //Entity Keys
    public static EntityCacheKey<List<Query>> AllQueries { get; } = CacheKey.CreateEntity<List<Query>>("AllQueries", true);

    public static EntityCacheKey<List<Instance>> AllInstances { get; } = CacheKey.CreateEntity<List<Instance>>("AllInstances", true);
}
