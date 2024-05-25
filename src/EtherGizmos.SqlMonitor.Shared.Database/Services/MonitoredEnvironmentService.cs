using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services;

/// <summary>
/// Provides access to <see cref="MonitoredEnvironment"/> records.
/// </summary>
internal class MonitoredEnvironmentService : IMonitoredEnvironmentService
{
    private readonly ApplicationContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public MonitoredEnvironmentService(ApplicationContext context)
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
