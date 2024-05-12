using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides access to <see cref="Query"/> records.
/// </summary>
internal class QueryService : IQueryService
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public QueryService(DatabaseContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public void Add(Query record)
    {
        if (!_context.Queries.Contains(record))
            _context.Queries.Add(record);
    }

    /// <inheritdoc/>
    public IQueryable<Query> GetQueryable()
    {
        return _context.Queries
            .Where(e => !e.IsSoftDeleted);
    }

    /// <inheritdoc/>
    public void Remove(Query record)
    {
        record.IsSoftDeleted = true;
    }
}
