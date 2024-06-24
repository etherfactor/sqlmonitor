using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Locking;

internal class InMemoryLockingCoordinator : ILockingCoordinator
{
    private Dictionary<string, bool> _consumedLocks = [];

    public async Task<CacheLock<TKey>?> AcquireLockAsync<TKey>(TKey key, TimeSpan timeout, CancellationToken cancellationToken = default) where TKey : ICacheKey
    {
        using var timeoutSource = new CancellationTokenSource(timeout);
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, cancellationToken);

        CacheLock<TKey>? @lock = null;
        do
        {
            lock (_consumedLocks)
            {
                if (!_consumedLocks.ContainsKey(key.Name))
                {
                    _consumedLocks.Add(key.Name, true);

                    @lock = new CacheLock<TKey>(key, new InMemorySynchronizationHandle(() =>
                    {
                        lock (_consumedLocks)
                        {
                            if (_consumedLocks.ContainsKey(key.Name))
                                _consumedLocks.Remove(key.Name);
                        }
                    }));
                }
            }

            if (@lock is null)
            {
                try
                {
                    await Task.Delay(100, timeoutSource.Token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        } while (@lock is null && !timeoutSource.IsCancellationRequested);

        return @lock;
    }

    private class InMemorySynchronizationHandle : Medallion.Threading.IDistributedSynchronizationHandle
    {
        private readonly Action _disposedCallback;
        private readonly CancellationTokenSource _handleLostTokenSource;

        public CancellationToken HandleLostToken => _handleLostTokenSource.Token;

        public InMemorySynchronizationHandle(Action disposedCallback)
        {
            _disposedCallback = disposedCallback;
            _handleLostTokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _handleLostTokenSource.Cancel();
            _disposedCallback();
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
