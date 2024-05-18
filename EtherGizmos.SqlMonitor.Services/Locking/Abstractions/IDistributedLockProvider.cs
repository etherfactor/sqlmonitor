namespace EtherGizmos.SqlMonitor.Services.Locking.Abstractions;

internal interface IDistributedLockProvider
{
    Task<CacheLock<TKey>?> AcquireLockAsync<TKey>(TKey key, TimeSpan timeout, CancellationToken cancellationToken = default) where TKey : ICacheKey;
}
