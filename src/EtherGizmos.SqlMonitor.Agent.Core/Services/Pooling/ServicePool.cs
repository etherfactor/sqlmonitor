using EtherGizmos.SqlMonitor.Agent.Core.Services.Pooling.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Pooling;

internal class ServicePool<TService> : IServicePool<TService>
    where TService : class
{
    private readonly ILogger? _logger = null;
    private readonly Func<Task<TService>> _serviceFactory;
    private readonly ConcurrentBag<TService> _pool;
    private readonly int _minPoolSize;
    private readonly int _maxPoolSize;
    private int _currentPoolSize;
    private readonly SemaphoreSlim _semaphore;
    private bool _disposed;

    public ServicePool(
        Func<Task<TService>> serviceFactory,
        int minPoolSize,
        int maxPoolSize,
        ILogger? logger = null)
    {
        _logger = logger;
        _serviceFactory = serviceFactory;
        _pool = new();
        _minPoolSize = minPoolSize;
        _maxPoolSize = maxPoolSize;
        _semaphore = new(maxPoolSize, maxPoolSize);

        for (int i = 0; i < _minPoolSize; i++)
        {
            _pool.Add(_serviceFactory().GetAwaiter().GetResult());
            Interlocked.Increment(ref _currentPoolSize);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _semaphore.Dispose();

                while (_pool.TryTake(out var service))
                {
                    (service as IDisposable)?.Dispose();
                    Interlocked.Decrement(ref _currentPoolSize);
                }
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async Task<ITicket<TService>> GetServiceAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);

        if (_disposed)
            throw new ObjectDisposedException(nameof(ServicePool<TService>));

        if (_pool.TryTake(out var service))
        {
            _logger?.LogInformation("Fetched service from pool ({CurrentPoolSize}/{MaxPoolSize})", _currentPoolSize, _maxPoolSize);
            return new Ticket(this, service);
        }
        else
        {
            var newService = await _serviceFactory();
            Interlocked.Increment(ref _currentPoolSize);
            _logger?.LogInformation("Created new service in pool ({CurrentPoolSize}/{MaxPoolSize})", _currentPoolSize, _maxPoolSize);
            return new Ticket(this, newService);
        }
    }

    private void ReturnService(TService service)
    {
        if (_disposed)
        {
            (service as IDisposable)?.Dispose();
            Interlocked.Decrement(ref _currentPoolSize);
        }
        else
        {
            _pool.Add(service);
            _semaphore.Release();
        }
    }

    private class Ticket : ITicket<TService>, IDisposable
    {
        private readonly ServicePool<TService> _pool;
        private bool _disposed;

        public TService Service { get; }

        public Ticket(
            ServicePool<TService> pool,
            TService service)
        {
            _pool = pool;
            Service = service;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _pool.ReturnService(Service);
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
