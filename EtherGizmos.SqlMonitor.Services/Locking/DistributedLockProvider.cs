using EtherGizmos.SqlMonitor.Services.Locking.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace EtherGizmos.SqlMonitor.Services.Locking;

internal class DistributedLockProvider : IDistributedLockProvider
{

    /// <inheritdoc/>
    public async Task<CacheLock<TKey>?> AcquireLockAsync<TKey>(TKey key, TimeSpan timeout, CancellationToken cancellationToken = default)
        where TKey : ICacheKey
    {
        var lockName = $"{Constants.Cache.SchemaName}:{key.KeyName}:{Constants.Cache.LockSuffix}";
        _logger.Log(LogLevel.Debug, "Attempting to acquire lock on {CacheKey}", lockName);

        var result = await _distributedLockProvider.TryAcquireLockAsync(lockName, timeout, cancellationToken);
        if (result is not null)
        {
            var keyLock = new CacheLock<TKey>(key, result);
            return keyLock;
        }

        return null;
    }

}
