using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;
using Medallion.Threading;
using Microsoft.Extensions.Logging;
using ILocalDistributedLockProvider = EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions.IDistributedLockProvider;
using IMedallionDistributedLockProvider = Medallion.Threading.IDistributedLockProvider;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Locking;

internal class DistributedLockProvider : ILocalDistributedLockProvider
{
    private readonly ILogger _logger;
    private readonly IMedallionDistributedLockProvider _medallionDistributedLockProvider;

    public DistributedLockProvider(
        ILogger<DistributedLockProvider> logger,
        IMedallionDistributedLockProvider medallionDistributedLockProvider)
    {
        _logger = logger;
        _medallionDistributedLockProvider = medallionDistributedLockProvider;
    }

    /// <inheritdoc/>
    public async Task<CacheLock<TKey>?> AcquireLockAsync<TKey>(TKey key, TimeSpan timeout, CancellationToken cancellationToken = default)
        where TKey : ICacheKey
    {
        var lockName = $"{ServiceConstants.Cache.SchemaName}:{ServiceConstants.Cache.LockPrefix}:{key.KeyName}";
        _logger.Log(LogLevel.Debug, "Attempting to acquire lock on {CacheKey}", lockName);

        var result = await _medallionDistributedLockProvider.TryAcquireLockAsync(lockName, timeout, cancellationToken);
        if (result is not null)
        {
            var keyLock = new CacheLock<TKey>(key, result);
            return keyLock;
        }

        return null;
    }
}
