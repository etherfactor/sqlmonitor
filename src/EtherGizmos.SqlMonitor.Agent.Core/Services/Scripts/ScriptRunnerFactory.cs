using EtherGizmos.SqlMonitor.Agent.Core.Extensions;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Communication.Abstractions;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Pooling;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Pooling.Abstractions;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Communication;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using System.Collections.Concurrent;
using System.Management.Automation.Runspaces;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts;

internal class ScriptRunnerFactory : IScriptRunnerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnectionRetriever _connectionRetriever;

    private readonly ConcurrentDictionary<int, Task<SshConfiguration>> _sshConfigurations = new();
    private readonly ConcurrentDictionary<int, IServicePool<SshClient>> _sshClients = new();

    private readonly ConcurrentDictionary<int, Task<WinRmConfiguration>> _winRmConfigurations = new();
    private readonly ConcurrentDictionary<int, IServicePool<Runspace>> _winRmClients = new();

    public ScriptRunnerFactory(
        IServiceProvider serviceProvider,
        IConnectionRetriever connectionRetriever)
    {
        _serviceProvider = serviceProvider;
        _connectionRetriever = connectionRetriever;
    }

    public async Task<IScriptRunner> GetRunnerAsync(int monitoredScriptTargetId, string connectionRequestToken, ExecType execType)
    {
        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();

        switch (execType)
        {
            case ExecType.Ssh:
                var sshConfigurationTask = _sshConfigurations.GetOrAdd(monitoredScriptTargetId, _ =>
                {
                    return _connectionRetriever.GetSshConfigurationAsync(connectionRequestToken);
                });

                SshConfiguration sshConfiguration;
                try
                {
                    sshConfiguration = await sshConfigurationTask;
                }
                catch
                {
                    _sshConfigurations.TryRemove(monitoredScriptTargetId, out var _);
                    throw;
                }

                var sshPool = _sshClients.GetOrAdd(monitoredScriptTargetId, _ =>
                {
                    var poolLogger = loggerFactory.CreateLogger<ServicePool<SshClient>>();
                    return new ServicePool<SshClient>(async () =>
                    {
                        var client = await sshConfiguration.CreateClientAsync();
                        return client;
                    }, 1, 8, logger: poolLogger);
                });

                var sshClient = await sshPool.GetServiceAsync();
                var sshLogger = loggerFactory.CreateLogger<SshScriptRunner>();
                var sshRunner = new SshScriptRunner(sshLogger, sshConfiguration, sshClient);

                return sshRunner;

            case ExecType.WinRm:
                var winRmConfigurationTask = _winRmConfigurations.GetOrAdd(monitoredScriptTargetId, _ =>
                {
                    return _connectionRetriever.GetWinRmConfigurationAsync(connectionRequestToken);
                });

                WinRmConfiguration winRmConfiguration;
                try
                {
                    winRmConfiguration = await winRmConfigurationTask;
                }
                catch
                {
                    _winRmConfigurations.TryRemove(monitoredScriptTargetId, out var _);
                    throw;
                }

                var winRmPool = _winRmClients.GetOrAdd(monitoredScriptTargetId, _ =>
                {
                    var poolLogger = loggerFactory.CreateLogger<ServicePool<SshClient>>();
                    return new ServicePool<Runspace>(async () =>
                    {
                        var runspace = await winRmConfiguration.CreateRunspaceAsync();
                        return runspace;
                    }, 1, 8, logger: poolLogger);
                });

                var winRmClient = await winRmPool.GetServiceAsync();
                var winRmLogger = loggerFactory.CreateLogger<PSRemotingScriptRunner>();
                var winRmRunner = new PSRemotingScriptRunner(winRmLogger, winRmConfiguration, winRmClient);

                return winRmRunner;

            default:
                throw new NotImplementedException();
        }
    }
}
