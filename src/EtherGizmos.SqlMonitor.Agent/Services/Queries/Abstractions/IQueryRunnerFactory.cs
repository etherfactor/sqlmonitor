namespace EtherGizmos.SqlMonitor.Agent.Services.Queries.Abstractions;

public interface IQueryRunnerFactory
{
    Task<IQueryRunner> GetRunnerAsync(int monitoredQueryTargetId, string connectionRequestToken, SqlType sqlType);
}
