namespace EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;

/// <summary>
/// Provides access to internal records.
/// </summary>
/// <typeparam name="T">The type of internal record.</typeparam>
public interface IQueryableService<T>
    where T : class
{
    /// <summary>
    /// Fetches the record set as a queryable.
    /// </summary>
    /// <returns>The record set as a queryable.</returns>
    IQueryable<T> GetQueryable();
}
