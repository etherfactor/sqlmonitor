using EtherGizmos.SqlMonitor.Api.Data.Access;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services;

/// <summary>
/// Provides access to <see cref="Securable"/> records.
/// </summary>
public class SecurableService : ISecurableService
{
    private DatabaseContext Context { get; }

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public SecurableService(DatabaseContext context)
    {
        Context = context;
    }

    /// <inheritdoc/>
    public async Task AddOrUpdate(Securable record)
    {
        if (!Context.Securables.Contains(record))
            Context.Securables.Add(record);

        await Context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public IQueryable<Securable> GetQueryable()
    {
        return Context.Securables;
    }
}
