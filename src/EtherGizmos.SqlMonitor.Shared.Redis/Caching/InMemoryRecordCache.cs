using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using Medallion.Threading;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching;

/// <summary>
/// Provides means to cache and retrieve records and record sets, in memory.
/// </summary>
internal class InMemoryRecordCache : IDistributedRecordCache
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

    private class InMemorySynchronizationHandle : IDistributedSynchronizationHandle
    {
        private readonly CancellationTokenSource _handleLostTokenSource;

        public CancellationToken HandleLostToken => _handleLostTokenSource.Token;

        public InMemorySynchronizationHandle()
        {
            _handleLostTokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _handleLostTokenSource.Cancel();
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
