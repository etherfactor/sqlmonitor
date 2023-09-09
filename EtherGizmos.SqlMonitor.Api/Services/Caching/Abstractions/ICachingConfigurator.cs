namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface ICachingConfigurator
{
    public IServiceCollection Services { get; }

    ICachingConfigurator UsingCache<TDistributedRecordCache>()
        where TDistributedRecordCache : class, IDistributedRecordCache;
}
