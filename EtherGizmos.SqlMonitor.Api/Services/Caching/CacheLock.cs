using Medallion.Threading;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching;

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
