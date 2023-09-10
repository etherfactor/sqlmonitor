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

        //**********************************************************
        // Instances

        //Load instances from the database and add them to the cache
        var instanceService = scope.GetRequiredService<IInstanceService>();
        var databaseInstances = await instanceService
            .GetQueryable()
            .ToListAsync();

        foreach (var instance in databaseInstances)
        {
            await _cache
                .EntitySet<Instance>()
                .AddAsync(instance);
        }

        //Load instances from the cache and delete the ones that don't exist in the database
        var cacheInstances = await _cache
            .EntitySet<Instance>()
            .ToListAsync();

        foreach (var instance in cacheInstances.Where(c => !databaseInstances.Any(d => d.Id == c.Id)))
        {
            await _cache
                .EntitySet<Instance>()
                .RemoveAsync(instance);
        }

        //**********************************************************
        // Queries

        //Load queries from the database and add them to the cache
        var queryService = scope.GetRequiredService<IQueryService>();
        var databaseQueries = await queryService
            .GetQueryable()
            .ToListAsync();

        foreach (var query in databaseQueries)
        {
            await _cache
                .EntitySet<Query>()
                .AddAsync(query);
        }

        //Load queries from the cache and delete the ones that don't exist in the database
        var cacheQueries = await _cache
            .EntitySet<Query>()
            .ToListAsync();

        foreach (var query in cacheQueries.Where(c => !databaseQueries.Any(d => d.Id == c.Id)))
        {
            await _cache
                .EntitySet<Query>()
                .RemoveAsync(query);
        }
    }
}
