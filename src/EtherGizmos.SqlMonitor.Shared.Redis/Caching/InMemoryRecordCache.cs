using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching;

/// <summary>
/// Provides means to cache and retrieve records and record sets, in memory.
/// </summary>
internal class InMemoryRecordCache : IRecordCache
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryRecordCache(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public ICacheEntity<TEntity> Entity<TEntity>(EntityCacheKey<TEntity> key)
        where TEntity : class, new()
    {
        return new InMemoryCacheEntity<TEntity>(_serviceProvider);
    }

    /// <inheritdoc/>
    public ICacheEntitySet<TEntity> EntitySet<TEntity>()
        where TEntity : class, new()
    {
        return new InMemoryCacheEntitySet<TEntity>(_serviceProvider);
    }
}
