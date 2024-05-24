using EtherGizmos.SqlMonitor.Agent.Services.Communication.Abstractions;
using EtherGizmos.SqlMonitor.Agent.Services.Queries.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;

namespace EtherGizmos.SqlMonitor.Agent.Services.Queries;

internal class QueryRunnerFactory : IQueryRunnerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnectionRetriever _connectionRetriever;

    private Dictionary<int, Task<IQueryRunner>> _runners = new();

    public QueryRunnerFactory(
        IServiceProvider serviceProvider,
        IConnectionRetriever connectionRetriever)
    {
        _serviceProvider = serviceProvider;
        _connectionRetriever = connectionRetriever;
    }

    public async Task<IQueryRunner> GetRunnerAsync(int monitoredQueryTargetId, string connectionRequestToken, SqlType sqlType)
    {
        lock (_runners)
        {
            if (!_runners.ContainsKey(monitoredQueryTargetId))
            {
                var task = LoadRunnerAsync(connectionRequestToken, sqlType);
                _runners.Add(monitoredQueryTargetId, task);
            }
        }

        return await _runners[monitoredQueryTargetId];
    }

    private async Task<IQueryRunner> LoadRunnerAsync(string connectionRequestToken, SqlType sqlType)
    {
        var connectionString = await _connectionRetriever.GetConnectionStringAsync(connectionRequestToken);

        return sqlType switch
        {
            SqlType.MariaDb => new MySqlQueryRunner(connectionString),
            SqlType.MySql => new MySqlQueryRunner(connectionString),
            SqlType.MicrosoftSqlServer => new SqlServerQueryRunner(connectionString),
            SqlType.PostgreSql => new PostgreSqlQueryRunner(connectionString),
            _ => throw new InvalidOperationException("Unrecognized SQL type"),
        };
    }
}
