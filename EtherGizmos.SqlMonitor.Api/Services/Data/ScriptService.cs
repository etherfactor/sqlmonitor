using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides access to <see cref="Script"/> records.
/// </summary>
internal class ScriptService : IEditableQueryableService<Script>, IScriptService
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public ScriptService(DatabaseContext context)
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
