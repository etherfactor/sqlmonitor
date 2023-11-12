using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

public class MetricBucketService : IMetricBucketService
{
    private readonly ILogger _logger;

    private readonly DatabaseContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="context">Provides access to internal records.</param>
    /// <param name="distributedRecordCache">Contains shared, cached records.</param>
    public MetricBucketService(
        ILogger<MetricBucketService> logger,
        DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <inheritdoc/>
    public void Add(MetricBucket record)
    {
        if (!_context.MetricBuckets.Contains(record))
            _context.MetricBuckets.Add(record);
    }

    /// <inheritdoc/>
    public IQueryable<MetricBucket> GetQueryable()
    {
        return _context.MetricBuckets;
    }

    /// <inheritdoc/>
    public void Remove(MetricBucket record)
    {
        throw new NotSupportedException();
    }
}
