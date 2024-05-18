
namespace EtherGizmos.SqlMonitor.Services.Locking;

internal interface IDistributedLockProvider
{
    Task<CacheLock<TKey>?> AcquireLockAsync<TKey>(TKey key, TimeSpan timeout, CancellationToken cancellationToken = default) where TKey : ICacheKey;
}