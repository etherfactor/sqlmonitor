using EtherGizmos.SqlMonitor.Agent.Core.Services.Communication.Abstractions;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Communication;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using System.Text.Json;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts;

internal class ScriptRunnerFactory : IScriptRunnerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnectionRetriever _connectionRetriever;

    private Dictionary<int, Task<IScriptRunner>> _runners = new();

    public ScriptRunnerFactory(
        IServiceProvider serviceProvider,
        IConnectionRetriever connectionRetriever)
    {
        _serviceProvider = serviceProvider;
        _connectionRetriever = connectionRetriever;
    }

    public async Task<IScriptRunner> GetRunnerAsync(int monitoredScriptTargetId, string connectionRequestToken, ExecType execType)
    {
        lock (_runners)
        {
            if (!_runners.ContainsKey(monitoredScriptTargetId))
            {
                var task = LoadRunnerAsync(connectionRequestToken, execType);
                _runners.Add(monitoredScriptTargetId, task);
            }
        }

        return await _runners[monitoredScriptTargetId];
    }

    private async Task<IScriptRunner> LoadRunnerAsync(string connectionRequestToken, ExecType execType)
    {
        var rawData = await _connectionRetriever.GetConnectionStringAsync(connectionRequestToken);

        switch (execType)
        {
            case ExecType.Ssh:
                var sshConfig = JsonSerializer.Deserialize<SshConfiguration>(rawData)
                    ?? throw new InvalidOperationException("Received malformed SSH config");
                return new SshScriptRunner(sshConfig);

            case ExecType.WinRm:
                var winRmConfig = JsonSerializer.Deserialize<WinRmConfiguration>(rawData)
                    ?? throw new InvalidOperationException("Received malformed WinRM config");
                return new PSRemotingScriptRunner(winRmConfig);

            default:
                throw new InvalidOperationException("Unrecognized exec type");
        };
    }
}
