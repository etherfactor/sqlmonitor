namespace EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;

public interface IDistributedLockProvider
{
    Task<CacheLock<TKey>?> AcquireLockAsync<TKey>(TKey key, TimeSpan timeout, CancellationToken cancellationToken = default) where TKey : ICacheKey;
}
