using EtherGizmos.SqlMonitor.Agent.Core.Services.Communication.Abstractions;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Pooling;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Pooling.Abstractions;
using EtherGizmos.SqlMonitor.Agent.Core.Services.Queries.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Npgsql;
using System.Collections.Concurrent;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Queries;

internal class QueryRunnerFactory : IQueryRunnerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnectionRetriever _connectionRetriever;

    private Dictionary<int, Task<IQueryRunner>> _runners = new();

    //Shared
    private readonly ConcurrentDictionary<int, Task<string>> _connectionStrings = new();

    //MySQL
    private readonly ConcurrentDictionary<int, IServicePool<MySqlConnection>> _mySqlConnections = new();

    //PostgreSQL
    private readonly ConcurrentDictionary<int, IServicePool<NpgsqlConnection>> _postgreSqlConnections = new();

    //SQL Server
    private readonly ConcurrentDictionary<int, IServicePool<SqlConnection>> _sqlServerConnections = new();

    public QueryRunnerFactory(
        IServiceProvider serviceProvider,
        IConnectionRetriever connectionRetriever)
    {
        _serviceProvider = serviceProvider;
        _connectionRetriever = connectionRetriever;
    }

    public async Task<IQueryRunner> GetRunnerAsync(int monitoredQueryTargetId, string connectionRequestToken, SqlType sqlType)
    {
        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();

        switch (sqlType)
        {
            case SqlType.MySql:
                var mySqlConnStrTask = _connectionStrings.GetOrAdd(monitoredQueryTargetId, _ =>
                {
                    return _connectionRetriever.GetConnectionStringAsync(connectionRequestToken);
                });

                string mySqlConnStr;
                try
                {
                    mySqlConnStr = await mySqlConnStrTask;
                }
                catch
                {
                    _connectionStrings.TryRemove(monitoredQueryTargetId, out var _);
                    throw;
                }

                var mySqlPool = _mySqlConnections.GetOrAdd(monitoredQueryTargetId, _ =>
                {
                    var poolLogger = loggerFactory.CreateLogger<ServicePool<MySqlConnection>>();
                    return new ServicePool<MySqlConnection>(async () =>
                    {
                        var connection = new MySqlConnection(mySqlConnStr);
                        await connection.OpenAsync();
                        return connection;
                    }, 1, 8, poolLogger);
                });

                var mySqlConn = await mySqlPool.GetServiceAsync();
                var mySqlLogger = loggerFactory.CreateLogger<MySqlQueryRunner>();
                var mySqlRunner = new MySqlQueryRunner(mySqlLogger, mySqlConn);

                return mySqlRunner;

            case SqlType.PostgreSql:
                var postgreSqlConnStrTask = _connectionStrings.GetOrAdd(monitoredQueryTargetId, _ =>
                {
                    return _connectionRetriever.GetConnectionStringAsync(connectionRequestToken);
                });

                string postgreSqlConnStr;
                try
                {
                    postgreSqlConnStr = await postgreSqlConnStrTask;
                }
                catch
                {
                    _connectionStrings.TryRemove(monitoredQueryTargetId, out var _);
                    throw;
                }

                var postgreSqlPool = _postgreSqlConnections.GetOrAdd(monitoredQueryTargetId, _ =>
                {
                    var poolLogger = loggerFactory.CreateLogger<ServicePool<MySqlConnection>>();
                    return new ServicePool<NpgsqlConnection>(async () =>
                    {
                        var connection = new NpgsqlConnection(postgreSqlConnStr);
                        await connection.OpenAsync();
                        return connection;
                    }, 1, 8, poolLogger);
                });

                var postgreSqlConn = await postgreSqlPool.GetServiceAsync();
                var postgreSqlLogger = loggerFactory.CreateLogger<PostgreSqlQueryRunner>();
                var postgreSqlRunner = new PostgreSqlQueryRunner(postgreSqlLogger, postgreSqlConn);

                return postgreSqlRunner;

            case SqlType.SqlServer:
                var sqlServerConnStrTask = _connectionStrings.GetOrAdd(monitoredQueryTargetId, _ =>
                {
                    return _connectionRetriever.GetConnectionStringAsync(connectionRequestToken);
                });

                string sqlServerConnStr;
                try
                {
                    sqlServerConnStr = await sqlServerConnStrTask;
                }
                catch
                {
                    _connectionStrings.TryRemove(monitoredQueryTargetId, out var _);
                    throw;
                }

                var sqlServerPool = _sqlServerConnections.GetOrAdd(monitoredQueryTargetId, _ =>
                {
                    var poolLogger = loggerFactory.CreateLogger<ServicePool<MySqlConnection>>();
                    return new ServicePool<SqlConnection>(async () =>
                    {
                        var connection = new SqlConnection(sqlServerConnStr);
                        await connection.OpenAsync();
                        return connection;
                    }, 1, 8, poolLogger);
                });

                var sqlServerConn = await sqlServerPool.GetServiceAsync();
                var sqlServerLogger = loggerFactory.CreateLogger<SqlServerQueryRunner>();
                var sqlServerRunner = new SqlServerQueryRunner(sqlServerLogger, sqlServerConn);

                return sqlServerRunner;

            default:
                throw new NotImplementedException();
        }
    }
}
