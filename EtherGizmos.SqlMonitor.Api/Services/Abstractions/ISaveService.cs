namespace EtherGizmos.SqlMonitor.Api.Services.Abstractions;

/// <summary>
/// Saves changes made by any/all <see cref="IQueryableService{T}"/>.
/// </summary>
public interface ISaveService
{
    /// <summary>
    /// Saves any/all changes made by <see cref="IQueryableService{T}"/>.
    /// </summary>
    /// <returns>An awaitable task.</returns>
    Task SaveChangesAsync();
}
