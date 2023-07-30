using EtherGizmos.SqlMonitor.Api.Data.Access;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Access;

/// <summary>
/// Provides access to <see cref="Instance"/> records.
/// </summary>
public class InstanceService : IInstanceService
{
    /// <summary>
    /// The name of the cache containing all records.
    /// </summary>
    public const string CacheName = "Instances";

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
    public InstanceService(ILogger<QueryService> logger, DatabaseContext context, IRecordCacheService cacheService)
    {
        Logger = logger;
        Context = context;
        CacheService = cacheService;
    }

    /// <inheritdoc/>
    public void AddOrUpdate(Instance record)
    {
        if (!Context.Instances.Contains(record))
            Context.Instances.Add(record);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Instance>> GetOrLoadCacheAsync()
    {
        return await CacheService.GetOrLoadCacheAsync(CacheName, async () =>
        {
            return await Context.Instances.ToListAsync();
        });
    }

    /// <inheritdoc/>
    public IQueryable<Instance> GetQueryable()
    {
        return Context.Instances;
    }
}
