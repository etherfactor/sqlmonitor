using EtherGizmos.SqlMonitor.Agent.Core.Services.Communication.Abstractions;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Queries.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Queries;

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

        try
        {
            return await _runners[monitoredQueryTargetId];
        }
        catch
        {
            lock (_runners)
            {
                _runners.Remove(monitoredQueryTargetId);
            }

            throw;
        }
    }

    private async Task<IQueryRunner> LoadRunnerAsync(string connectionRequestToken, SqlType sqlType)
    {
        var connectionString = await _connectionRetriever.GetConnectionStringAsync(connectionRequestToken);

        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();

        return sqlType switch
        {
            SqlType.MariaDb => new MySqlQueryRunner(
                loggerFactory.CreateLogger<MySqlQueryRunner>(),
                connectionString),

            SqlType.MySql => new MySqlQueryRunner(
                loggerFactory.CreateLogger<MySqlQueryRunner>(),
                connectionString),

            SqlType.SqlServer => new SqlServerQueryRunner(
                loggerFactory.CreateLogger<SqlServerQueryRunner>(),
                connectionString),

            SqlType.PostgreSql => new PostgreSqlQueryRunner(
                loggerFactory.CreateLogger<PostgreSqlQueryRunner>(),
                connectionString),

            _ => throw new InvalidOperationException("Unrecognized SQL type"),
        };
    }
}
