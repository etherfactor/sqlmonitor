using EtherGizmos.SqlMonitor.Shared.Models.Communication;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Communication.Abstractions;

public interface IConnectionRetriever
{
    Task<string> GetConnectionStringAsync(string connectionToken, CancellationToken cancellationToken = default);
    Task<SshConfiguration> GetSshConfigurationAsync(string connectionToken, CancellationToken cancellationToken = default);
    Task<WinRmConfiguration> GetWinRmConfigurationAsync(string connectionToken, CancellationToken cancellationToken = default);
}
