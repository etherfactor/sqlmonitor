using EtherGizmos.SqlMonitor.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides access to <see cref="Metric"/> records.
/// </summary>
public class MetricService : IMetricService
{
    private readonly ApplicationContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public MetricService(ApplicationContext context)
    {
        _context = context;
    }

    public void Add(Metric record)
    {
        if (!_context.Metrics.Contains(record))
            _context.Metrics.Add(record);
    }

    public IQueryable<Metric> GetQueryable()
    {
        return _context.Metrics.Where(e => !e.IsSoftDeleted);
    }

    public void Remove(Metric record)
    {
        record.IsSoftDeleted = true;
    }
}
