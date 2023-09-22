using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides access to <see cref="Metric"/> records.
/// </summary>
public class MetricService : IMetricService
{
    private readonly ILogger _logger;
    private readonly DatabaseContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="databaseContext">Provides access to internal records.</param>
    public MetricService(
        ILogger<MetricService> logger,
        DatabaseContext databaseContext)
    {
        _logger = logger;
        _context = databaseContext;
    }

    /// <inheritdoc/>
    public void Add(Metric record)
    {
        if (!_context.Metrics.Contains(record))
            _context.Metrics.Add(record);
    }

    /// <inheritdoc/>
    public IQueryable<Metric> GetQueryable()
    {
        return _context.Metrics;
    }

    /// <inheritdoc/>
    public void Remove(Metric record)
    {
        throw new NotSupportedException("Deleting metrics is currently not supported.");
    }
}
