using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services;

/// <summary>
/// Provides access to <see cref="MonitoredTargetMetricBySecond"/> records.
/// </summary>
internal class MonitoredTargetMetricsBySecondService : IMonitoredTargetMetricsBySecondService
{
    private readonly ApplicationContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public MonitoredTargetMetricsBySecondService(ApplicationContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public void Add(MonitoredTargetMetricBySecond record)
    {
        if (!_context.MonitoredTargetMetricsBySecond.Contains(record))
            _context.MonitoredTargetMetricsBySecond.Add(record);
        else
            _context.MonitoredTargetMetricsBySecond.Attach(record);
    }

    /// <inheritdoc/>
    public IQueryable<MonitoredTargetMetricBySecond> GetQueryable()
    {
        return _context.MonitoredTargetMetricsBySecond;
    }

    /// <inheritdoc/>
    public void Remove(MonitoredTargetMetricBySecond record)
    {
        throw new NotSupportedException();
    }
}
