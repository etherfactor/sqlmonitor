using EtherGizmos.SqlMonitor.Api.Services.Background.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Services.Background;

public class CacheLoadService : OneTimeBackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDistributedRecordCache _cache;

    public CacheLoadService(
        ILogger<CacheLoadService> logger,
        IServiceProvider serviceProvider,
        IDistributedRecordCache distributedRecordCache)
        : base(logger)
    {
        _serviceProvider = serviceProvider;
        _cache = distributedRecordCache;
    }

    /// <inheritdoc/>
    protected override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        var scope = _serviceProvider.CreateScope()
            .ServiceProvider;

        var instanceService = scope.GetRequiredService<IInstanceService>();
        var instances = await instanceService
            .GetQueryable()
            .ToListAsync();

        foreach (var instance in instances)
        {
            await _cache
                .EntitySet<Instance>()
                .AddAsync(instance);
        }

        var queryService = scope.GetRequiredService<IQueryService>();
        var queries = await queryService
            .GetQueryable()
            .ToListAsync();

        foreach (var query in queries)
        {
            await _cache
                .EntitySet<Query>()
                .AddAsync(query);
        }
    }
}
