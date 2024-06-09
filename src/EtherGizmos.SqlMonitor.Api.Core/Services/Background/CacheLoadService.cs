using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Services.Background.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtherGizmos.SqlMonitor.Api.Core.Services.Background;

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
        // Queries

        //Load queries from the database and add them to the cache
        var queryService = scope.GetRequiredService<IQueryService>();
        var databaseQueries = await queryService
            .GetQueryable()
            .ToListAsync(stoppingToken);

        foreach (var query in databaseQueries)
        {
            await _cache
                .EntitySet<Query>()
                .AddAsync(query, stoppingToken);
        }

        //Load queries from the cache and delete the ones that don't exist in the database
        var cacheQueries = await _cache
            .EntitySet<Query>()
            .ToListAsync(stoppingToken);

        foreach (var query in cacheQueries.Where(c => !databaseQueries.Any(d => d.Id == c.Id)))
        {
            await _cache
                .EntitySet<Query>()
                .RemoveAsync(query, stoppingToken);
        }

        //**********************************************************
        // Monitored query targets

        //Load queries from the database and add them to the cache
        var queryTargetService = scope.GetRequiredService<IMonitoredQueryTargetService>();
        var databaseQueryTargets = await queryTargetService
            .GetQueryable()
            .ToListAsync(stoppingToken);

        foreach (var queryTarget in databaseQueryTargets)
        {
            await _cache
                .EntitySet<MonitoredQueryTarget>()
                .AddAsync(queryTarget, stoppingToken);
        }

        //Load queries from the cache and delete the ones that don't exist in the database
        var cacheQueryTargets = await _cache
            .EntitySet<MonitoredQueryTarget>()
            .ToListAsync(stoppingToken);

        foreach (var queryTarget in cacheQueryTargets.Where(c => !databaseQueryTargets.Any(d => d.Id == c.Id)))
        {
            await _cache
                .EntitySet<MonitoredQueryTarget>()
                .RemoveAsync(queryTarget, stoppingToken);
        }

        //**********************************************************
        // Scripts

        //Load scripts from the database and add them to the cache
        var scriptService = scope.GetRequiredService<IScriptService>();
        var databaseScripts = await scriptService
            .GetQueryable()
            .ToListAsync(stoppingToken);

        foreach (var script in databaseScripts)
        {
            await _cache
                .EntitySet<Script>()
                .AddAsync(script, stoppingToken);
        }

        //Load scripts from the cache and delete the ones that don't exist in the database
        var cacheScripts = await _cache
            .EntitySet<Script>()
            .ToListAsync(stoppingToken);

        foreach (var script in cacheScripts.Where(c => !databaseScripts.Any(d => d.Id == c.Id)))
        {
            await _cache
                .EntitySet<Script>()
                .RemoveAsync(script, stoppingToken);
        }

        //**********************************************************
        // Monitored script targets

        //Load scripts from the database and add them to the cache
        var scriptTargetService = scope.GetRequiredService<IMonitoredScriptTargetService>();
        var databaseScriptTargets = await scriptTargetService
            .GetQueryable()
            .ToListAsync(stoppingToken);

        foreach (var scriptTarget in databaseScriptTargets)
        {
            await _cache
                .EntitySet<MonitoredScriptTarget>()
                .AddAsync(scriptTarget, stoppingToken);
        }

        //Load scripts from the cache and delete the ones that don't exist in the database
        var cacheScriptTargets = await _cache
            .EntitySet<MonitoredScriptTarget>()
            .ToListAsync(stoppingToken);

        foreach (var scriptTarget in cacheScriptTargets.Where(c => !databaseScriptTargets.Any(d => d.Id == c.Id)))
        {
            await _cache
                .EntitySet<MonitoredScriptTarget>()
                .RemoveAsync(scriptTarget, stoppingToken);
        }
    }
}
