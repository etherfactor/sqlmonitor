using Medallion.Threading;
using Microsoft.Extensions.Caching.Distributed;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Storage;

public interface ILockedDistributedCache
{
    Task<bool> TryAcquireLockAsync<TEntity>(CacheKey<TEntity> key, TimeSpan timeout, Action<CacheLock<TEntity>> @out, CancellationToken cancellationToken = default);

    Task<bool> TryGetAsync<TEntity>(CacheKey<TEntity> key, Action<TEntity?> @out, CancellationToken cancellationToken = default);

    Task<bool> TryRefreshAsync<TEntity>(CacheKey<TEntity> key, TEntity record, CancellationToken cancellationToken = default);

    Task<bool> TrySetAsync<TEntity>(CacheKey<TEntity> key, TEntity record, DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default);

    Task<bool> TrySetWithLockAsync<TEntity>(CacheKey<TEntity> key, CacheLock<TEntity> keyLock, TEntity record, DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default);

    Task<bool> TryRemoveAsync<TEntity>(CacheKey<TEntity> key, CancellationToken cancellationToken = default);
}

public static class CacheKey
{
    private static Dictionary<string, object> Keys { get; } = new Dictionary<string, object>();
    private static Dictionary<string, Type> KeyTypes { get; } = new Dictionary<string, Type>();
    private static Dictionary<string, int> KeyHashes { get; } = new Dictionary<string, int>();

    public static CacheKey<TEntity> Create<TEntity>(string key, bool requiresLock)
    {
        var newCacheKey = new CacheKey<TEntity>(key, requiresLock);
        if (Keys.ContainsKey(key))
        {
            if (KeyTypes[key] != typeof(TEntity) || KeyHashes[key] != newCacheKey.GetHashCode())
                throw new InvalidOperationException($"The cache key {key} already exists.");
        }
        else
        {
            Keys.Add(key, newCacheKey);
            KeyTypes.Add(key, typeof(TEntity));
            KeyHashes.Add(key, newCacheKey.GetHashCode());
        }

        var cacheKey = (CacheKey<TEntity>)Keys[key];

        return cacheKey;
    }
}

public struct CacheKey<TEntity>
{
    public readonly string Name { get; }

    public readonly bool RequiresLock { get; }

    internal CacheKey(string name, bool requiresLock)
    {
        Name = name;
        RequiresLock = requiresLock;
    }

    public override int GetHashCode()
    {
        return (Name, RequiresLock).GetHashCode();
    }
}

public class CacheRecord<TEntity>
{
    public CacheKey<TEntity> Key { get; }

    public TEntity Value { get; }

    public DateTimeOffset? ExpiresAt { get; }

    public CacheRecord(CacheKey<TEntity> key, TEntity entity, DateTimeOffset? expiresAt = null)
    {
        Key = key;
        Value = entity;
        ExpiresAt = expiresAt;
    }
}

public class CacheLock<TEntity> : IDisposable
{
    private bool _disposedValue;
    private IDistributedSynchronizationHandle _distributedLock;

    public CacheKey<TEntity> Key { get; }

    public CancellationToken HandleLostToken => _distributedLock.HandleLostToken;

    public bool IsValid => !HandleLostToken.IsCancellationRequested;

    internal CacheLock(CacheKey<TEntity> key, IDistributedSynchronizationHandle distributedLock)
    {
        Key = key;
        _distributedLock = distributedLock;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _distributedLock.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        //Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
