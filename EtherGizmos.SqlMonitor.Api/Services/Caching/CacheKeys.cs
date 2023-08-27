using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

public static class CacheKeys
{
    public static CacheKey<List<Query>> AllQueries { get; } = CacheKey.Create<List<Query>>("AllQueries", true);

    public static CacheKey<List<Instance>> AllInstances { get; } = CacheKey.Create<List<Instance>>("AllInstances", true);
}
