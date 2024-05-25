using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Agent.Services.Queries.Abstractions;

public interface IQueryRunnerFactory
{
    Task<IQueryRunner> GetRunnerAsync(int monitoredQueryTargetId, string connectionRequestToken, SqlType sqlType);
}
