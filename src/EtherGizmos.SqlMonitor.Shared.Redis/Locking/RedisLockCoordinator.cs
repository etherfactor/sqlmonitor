using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;
using Medallion.Threading;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Locking;

internal class RedisLockCoordinator : ILockingCoordinator
{
    private readonly ILogger _logger;
    private readonly IDistributedLockProvider _medallionDistributedLockProvider;

    public RedisLockCoordinator(
        ILogger<RedisLockCoordinator> logger,
        IDistributedLockProvider medallionDistributedLockProvider)
    {
        _logger = logger;
        _medallionDistributedLockProvider = medallionDistributedLockProvider;
    }

    /// <inheritdoc/>
    public async Task<CacheLock<TKey>?> AcquireLockAsync<TKey>(TKey key, TimeSpan timeout, CancellationToken cancellationToken = default)
        where TKey : ICacheKey
    {
        var lockName = $"{ServiceConstants.Cache.SchemaName}:{ServiceConstants.Cache.LockPrefix}:{key.Name}";
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
