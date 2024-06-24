namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Pooling.Abstractions;

public interface ITicket<TService> : IDisposable
{
    public TService Service { get; }
}
