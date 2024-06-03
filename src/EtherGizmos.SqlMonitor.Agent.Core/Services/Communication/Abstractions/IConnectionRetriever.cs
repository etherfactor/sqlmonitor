namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Communication.Abstractions;

public interface IConnectionRetriever
{
    Task<string> GetConnectionStringAsync(string connectionToken, CancellationToken cancellationToken = default);
}
