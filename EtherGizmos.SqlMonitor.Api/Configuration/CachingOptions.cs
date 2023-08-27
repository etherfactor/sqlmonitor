using Microsoft.Extensions.Caching.Distributed;

namespace EtherGizmos.SqlMonitor.Api.Configuration;

public class CachingOptions
{
    public Dictionary<string, DistributedCacheEntryOptions> Keys { get; set; } = new();
}
