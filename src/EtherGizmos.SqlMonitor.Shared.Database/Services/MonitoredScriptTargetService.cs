using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services;

/// <summary>
/// Provides access to <see cref="MonitoredScriptTarget"/> records.
/// </summary>
public class MonitoredScriptTargetService : IMonitoredScriptTargetService
{
    private readonly ApplicationContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public MonitoredScriptTargetService(ApplicationContext context)
    {
        _context = context;
    }

    public void Add(MonitoredScriptTarget record)
    {
        if (!_context.MonitoredScriptTargets.Contains(record))
            _context.MonitoredScriptTargets.Add(record);
        else
            _context.MonitoredScriptTargets.Attach(record);
    }

    public IQueryable<MonitoredScriptTarget> GetQueryable()
    {
        return _context.MonitoredScriptTargets;
    }

    public void Remove(MonitoredScriptTarget record)
    {
        _context.MonitoredScriptTargets.Remove(record);
    }
}
