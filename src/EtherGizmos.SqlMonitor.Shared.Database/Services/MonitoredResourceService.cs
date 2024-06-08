using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services;

/// <summary>
/// Provides access to <see cref="MonitoredResource"/> records.
/// </summary>
internal class MonitoredResourceService : IMonitoredResourceService
{
    private readonly ApplicationContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public MonitoredResourceService(ApplicationContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public void Add(MonitoredResource record)
    {
        if (!_context.MonitoredResources.Contains(record))
            _context.MonitoredResources.Add(record);
        else
            _context.MonitoredResources.Attach(record);
    }

    /// <inheritdoc/>
    public IQueryable<MonitoredResource> GetQueryable()
    {
        return _context.MonitoredResources
            .Where(e => !e.IsSoftDeleted);
    }

    /// <inheritdoc/>
    public void Remove(MonitoredResource record)
    {
        record.IsSoftDeleted = true;
    }
}
