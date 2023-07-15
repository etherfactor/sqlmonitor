using EtherGizmos.SqlMonitor.Api.Data.Access;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Access;

/// <summary>
/// Saves changes made by any/all <see cref="IEditableQueryableService{T}"/>.
/// </summary>
public class SaveService : ISaveService
{
    /// <summary>
    /// The EF context.
    /// </summary>
    private DatabaseContext Context { get; }

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The EF context.</param>
    public SaveService(DatabaseContext context)
    {
        Context = context;
    }

    /// <inheritdoc/>
    public async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }
}
