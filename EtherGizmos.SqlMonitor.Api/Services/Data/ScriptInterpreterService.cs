using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides access to <see cref="ScriptInterpreter"/> records.
/// </summary>
public class ScriptInterpreterService : IScriptInterpreterService
{
    private readonly ApplicationContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public ScriptInterpreterService(ApplicationContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public void Add(ScriptInterpreter record)
    {
        if (!_context.ScriptInterpreters.Contains(record))
            _context.ScriptInterpreters.Add(record);
    }

    /// <inheritdoc/>
    public IQueryable<ScriptInterpreter> GetQueryable()
    {
        return _context.ScriptInterpreters
            .Where(e => !e.IsSoftDeleted);
    }

    /// <inheritdoc/>
    public void Remove(ScriptInterpreter record)
    {
        record.IsSoftDeleted = true;
    }
}
