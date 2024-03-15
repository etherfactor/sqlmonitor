using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides access to <see cref="MonitoredEnvironment"/> records.
/// </summary>
public class MonitoredEnvironmentService : IMonitoredEnvironmentService
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public MonitoredEnvironmentService(DatabaseContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public void Add(MonitoredEnvironment record)
    {
        if (!_context.MonitoredEnvironments.Contains(record))
            _context.MonitoredEnvironments.Add(record);
    }

    /// <inheritdoc/>
    public IQueryable<MonitoredEnvironment> GetQueryable()
    {
        return _context.MonitoredEnvironments;
    }

    /// <inheritdoc/>
    public void Remove(MonitoredEnvironment record)
    {
        record.IsSoftDeleted = true;
    }
}
