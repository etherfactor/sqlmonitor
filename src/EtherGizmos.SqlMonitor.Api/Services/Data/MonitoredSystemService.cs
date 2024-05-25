using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

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
