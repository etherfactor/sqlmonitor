using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services;

/// <summary>
/// Provides access to <see cref="MonitoredSystem"/> records.
/// </summary>
internal class MonitoredSystemService : IMonitoredSystemService
{
    private readonly ApplicationContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public MonitoredSystemService(ApplicationContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public void Add(MonitoredSystem record)
    {
        if (!_context.MonitoredSystems.Contains(record))
            _context.MonitoredSystems.Add(record);
        else
            _context.MonitoredSystems.Attach(record);
    }

    /// <inheritdoc/>
    public IQueryable<MonitoredSystem> GetQueryable()
    {
        return _context.MonitoredSystems
            .Where(e => !e.IsSoftDeleted);
    }

    /// <inheritdoc/>
    public void Remove(MonitoredSystem record)
    {
        record.IsSoftDeleted = true;
    }
}
