using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Scripts.Abstractions;

public interface IScriptRunnerFactory
{
    Task<IScriptRunner> GetRunnerAsync(int monitoredScriptTargetId, string connectionRequestToken, ExecType execType);
}