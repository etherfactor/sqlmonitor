using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Services.Background.Abstractions;
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
