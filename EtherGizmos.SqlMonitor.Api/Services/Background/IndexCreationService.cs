using EtherGizmos.SqlMonitor.Models.Database;
using Redis.OM;
using Redis.OM.Contracts;

namespace EtherGizmos.SqlMonitor.Api.Services.Background;

public class IndexCreationService : IHostedService
{
    private readonly IRedisConnectionProvider _connectionProvider;

    public IndexCreationService(IRedisConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _connectionProvider.Connection.CreateIndexAsync(typeof(Instance));
        await _connectionProvider.Connection.CreateIndexAsync(typeof(Query));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
