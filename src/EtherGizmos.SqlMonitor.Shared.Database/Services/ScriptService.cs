using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services;

/// <summary>
/// Provides access to <see cref="Script"/> records.
/// </summary>
internal class ScriptService : IEditableQueryableService<Script>, IScriptService
{
    private readonly ApplicationContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public ScriptService(ApplicationContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public void Add(Script record)
    {
        if (!_context.Scripts.Contains(record))
            _context.Scripts.Add(record);
    }

    /// <inheritdoc/>
    public IQueryable<Script> GetQueryable()
    {
        return _context.Scripts
            .Where(e => !e.IsSoftDeleted);
    }

    /// <inheritdoc/>
    public void Remove(Script record)
    {
        record.IsSoftDeleted = true;
    }
}
