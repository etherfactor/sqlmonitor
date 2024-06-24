using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services;

/// <summary>
/// Provides access to <see cref="MonitoredQueryTarget"/> records.
/// </summary>
public class MonitoredQueryTargetService : IMonitoredQueryTargetService
{
    private readonly ApplicationContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public MonitoredQueryTargetService(ApplicationContext context)
    {
        _context = context;
    }

    public void Add(MonitoredQueryTarget record)
    {
        if (!_context.MonitoredQueryTargets.Contains(record))
            _context.MonitoredQueryTargets.Add(record);
        else
            _context.MonitoredQueryTargets.Attach(record);
    }

    public IQueryable<MonitoredQueryTarget> GetQueryable()
    {
        return _context.MonitoredQueryTargets;
    }

    public void Remove(MonitoredQueryTarget record)
    {
        _context.MonitoredQueryTargets.Remove(record);
    }
}
