﻿using EtherGizmos.SqlMonitor.Api.Services.Background.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

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
    protected internal override async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        var scope = _serviceProvider.CreateScope()
            .ServiceProvider;

        //**********************************************************
        // Queries

        ////Load queries from the database and add them to the cache
        //var queryService = scope.GetRequiredService<IQueryService>();
        //var databaseQueries = await queryService
        //    .GetQueryable()
        //    .ToListAsync();

        //foreach (var query in databaseQueries)
        //{
        //    await _cache
        //        .EntitySet<Query>()
        //        .AddAsync(query);
        //}

        ////Load queries from the cache and delete the ones that don't exist in the database
        //var cacheQueries = await _cache
        //    .EntitySet<Query>()
        //    .ToListAsync();

        //foreach (var query in cacheQueries.Where(c => !databaseQueries.Any(d => d.Id == c.Id)))
        //{
        //    await _cache
        //        .EntitySet<Query>()
        //        .RemoveAsync(query);
        //}
    }
}
