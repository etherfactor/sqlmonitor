using Medallion.Threading;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

/// <summary>
/// Holds a lock on a key.
/// </summary>
/// <typeparam name="TKey">The key that is locked.</typeparam>
public class CacheLock<TKey> : IDisposable
    where TKey : ICacheKey
{
    private bool _disposedValue;
    private IDistributedSynchronizationHandle _distributedLock;

    /// <summary>
    /// The key that is locked.
    /// </summary>
    public TKey Key { get; }

    /// <summary>
    /// A token that is cancelled when the lock prematurely expires.
    /// </summary>
    public CancellationToken LostLockCancellationToken => _distributedLock.HandleLostToken;

    /// <summary>
    /// Indicates if the lock is still valid. May incur additional costs to verify the validity of the lock.
    /// </summary>
    public bool IsValid => !LostLockCancellationToken.IsCancellationRequested;

    internal CacheLock(TKey key, IDistributedSynchronizationHandle distributedLock)
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

    /// <inheritdoc/>
    public void Dispose()
    {
        //Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
