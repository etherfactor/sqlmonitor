namespace EtherGizmos.SqlMonitor.Agent.Services.Communication.Abstractions;

public interface IConnectionRetriever
{
    Task<string> GetConnectionStringAsync(string connectionToken, CancellationToken cancellationToken = default);
}
