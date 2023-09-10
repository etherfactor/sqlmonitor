using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Caching;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides access to <see cref="Instance"/> records.
/// </summary>
public class InstanceService : IInstanceService
{
    private readonly ILogger _logger;

    private readonly DatabaseContext _context;
    private readonly IDistributedRecordCache _distributedRecordCache;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="context">Provides access to internal records.</param>
    /// <param name="distributedRecordCache">Contains shared, cached records.</param>
    public InstanceService(
        ILogger<QueryService> logger,
        DatabaseContext context,
        IDistributedRecordCache distributedRecordCache)
    {
        _logger = logger;
        _context = context;
        _distributedRecordCache = distributedRecordCache;
    }

    /// <inheritdoc/>
    public void Add(Instance record)
    {
        if (!_context.Instances.Contains(record))
            _context.Instances.Add(record);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Instance>> GetOrLoadCacheAsync(CancellationToken cancellationToken = default)
    {
        return await _distributedRecordCache.GetOrCalculateAsync(CacheKeys.AllInstances, async () =>
        {
            return await _context.Instances.ToListAsync();
        }, timeout: TimeSpan.FromSeconds(15), cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public IQueryable<Instance> GetQueryable()
    {
        return _context.Instances;
    }

    /// <inheritdoc/>
    public void Remove(Instance record)
    {
        record.IsActive = false;
        record.IsSoftDeleted = true;
    }
}
