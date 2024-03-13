using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides access to <see cref="MonitoredSystem"/> records.
/// </summary>
public class MonitoredSystemService : IMonitoredSystemService
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public MonitoredSystemService(DatabaseContext context)
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
        return _context.MonitoredSystems;
    }

    /// <inheritdoc/>
    public void Remove(MonitoredSystem record)
    {
        record.IsSoftDeleted = true;
    }
}
