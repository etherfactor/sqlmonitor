namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Pooling.Abstractions;

public interface IServicePool<TService> : IDisposable
{
    Task<ITicket<TService>> GetServiceAsync(CancellationToken cancellationToken = default);
}
