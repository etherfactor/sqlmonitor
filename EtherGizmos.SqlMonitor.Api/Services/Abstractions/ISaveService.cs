namespace EtherGizmos.SqlMonitor.Api.Services.Abstractions;

/// <summary>
/// Saves changes made by any/all <see cref="IEditableQueryableService{T}"/>.
/// </summary>
public interface ISaveService
{
    /// <summary>
    /// Saves any/all changes made by <see cref="IEditableQueryableService{T}"/>.
    /// </summary>
    /// <returns>An awaitable task.</returns>
    Task SaveChangesAsync();

    /// <summary>
    /// Associates a record in such a way as to allow <see cref="SaveChangesAsync"/> to apply.
    /// </summary>
    /// <param name="record"></param>
    void Attach(object record);
}
