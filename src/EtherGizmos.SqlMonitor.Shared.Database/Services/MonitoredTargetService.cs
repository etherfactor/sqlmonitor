using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services;

/// <summary>
/// Provides access to <see cref="MonitoredTarget"/> records.
/// </summary>
internal class MonitoredTargetService : IMonitoredTargetService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ApplicationContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public MonitoredTargetService(
        IServiceProvider serviceProvider,
        ApplicationContext context)
    {
        _serviceProvider = serviceProvider;
        _context = context;
    }

    /// <inheritdoc/>
    public void Add(MonitoredTarget record)
    {
        if (!_context.MonitoredTargets.Contains(record))
            _context.MonitoredTargets.Add(record);
        else
            _context.MonitoredTargets.Attach(record);
    }

    /// <inheritdoc/>
    public async Task<MonitoredTarget> GetOrCreateAsync(Guid systemId, Guid resourceId, Guid environmentId, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var provider = scope.ServiceProvider;

        var lockFactory = provider.GetRequiredService<IMonitoredTargetLockFactory>();
        var coordinator = provider.GetRequiredService<ILockingCoordinator>();
        var saveService = provider.GetRequiredService<ISaveService>();
        var monitoredTargetService = provider.GetRequiredService<IMonitoredTargetService>();

        var maybeTarget = await monitoredTargetService.GetQueryable()
            .SingleOrDefaultAsync(e => e.MonitoredSystemId == systemId &&
                e.MonitoredResourceId == resourceId &&
                e.MonitoredEnvironmentId == environmentId);

        if (maybeTarget is null)
        {
            var key = lockFactory.CreateKey(systemId, resourceId, environmentId);
            using var @lock = await coordinator.AcquireLockAsync(key, TimeSpan.FromDays(1), cancellationToken)
                ?? throw new TimeoutException("Failed to acquire lock");

            maybeTarget = await monitoredTargetService.GetQueryable()
                .SingleOrDefaultAsync(e => e.MonitoredSystemId == systemId &&
                    e.MonitoredResourceId == resourceId &&
                    e.MonitoredEnvironmentId == environmentId);

            if (maybeTarget is null)
            {
                maybeTarget = new()
                {
                    MonitoredSystemId = systemId,
                    MonitoredResourceId = resourceId,
                    MonitoredEnvironmentId = environmentId,
                };

                monitoredTargetService.Add(maybeTarget);
                await saveService.SaveChangesAsync();
            }
        }

        //Refresh the record, in this context
        return await GetQueryable()
            .SingleAsync(e => e.Id == maybeTarget.Id);
    }

    /// <inheritdoc/>
    public IQueryable<MonitoredTarget> GetQueryable()
    {
        return _context.MonitoredTargets;
    }

    /// <inheritdoc/>
    public void Remove(MonitoredTarget record)
    {
        throw new NotSupportedException();
    }
}
