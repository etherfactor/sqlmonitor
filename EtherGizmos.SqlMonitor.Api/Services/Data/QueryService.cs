using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides access to <see cref="Query"/> records.
/// </summary>
public class QueryService : IQueryService
{
    private readonly ILogger _logger;
    private readonly DatabaseContext _context;
    private readonly ILockedDistributedCache _distributedCache;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="context">Provides access to internal records.</param>
    /// <param name="distributedCache">Contains shared, cached records.</param>
    public QueryService(
        ILogger<QueryService> logger,
        DatabaseContext context,
        ILockedDistributedCache distributedCache)
    {
        _logger = logger;
        _context = context;
        _distributedCache = distributedCache;
    }

    /// <inheritdoc/>
    public void Add(Query record)
    {
        if (!_context.Queries.Contains(record))
            _context.Queries.Add(record);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Query>> GetOrLoadCacheAsync(CancellationToken cancellationToken = default)
    {
        return await _distributedCache.GetOrCalculateAsync(CacheKeys.AllQueries, async () =>
        {
            return await GetQueryable().ToListAsync();
        }, timeout: TimeSpan.FromSeconds(15), cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public IQueryable<Query> GetQueryable()
    {
        return _context.Queries.Where(e => !e.IsSoftDeleted);
    }

    /// <inheritdoc/>
    public void Remove(Query record)
    {
        record.IsSoftDeleted = true;
    }
}
