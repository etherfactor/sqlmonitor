using EtherGizmos.SqlMonitor.Api.Data.Access;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Access;

/// <summary>
/// Provides access to <see cref="Query"/> records.
/// </summary>
public class QueryService : IQueryService
{
    /// <summary>
    /// The name of the cache containing all records.
    /// </summary>
    public const string CacheName = "Queries";

    /// <summary>
    /// The logger to use.
    /// </summary>
    private ILogger Logger { get; }

    /// <summary>
    /// Provides access to internal records.
    /// </summary>
    private DatabaseContext Context { get; }

    /// <summary>
    /// Contains shared, cached records.
    /// </summary>
    private IRecordCacheService CacheService { get; }

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="context">Provides access to internal records.</param>
    /// <param name="cacheService">Contains shared, cached records.</param>
    public QueryService(ILogger<QueryService> logger, DatabaseContext context, IRecordCacheService cacheService)
    {
        Logger = logger;
        Context = context;
        CacheService = cacheService;
    }

    /// <inheritdoc/>
    public void AddOrUpdate(Query record)
    {
        if (!Context.Queries.Contains(record))
            Context.Queries.Add(record);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Query>> GetOrLoadCacheAsync()
    {
        return await CacheService.GetOrLoadCacheAsync(CacheName, async () =>
        {
            return await GetQueryable().ToListAsync();
        });
    }

    /// <inheritdoc/>
    public IQueryable<Query> GetQueryable()
    {
        return Context.Queries;
    }
}
