namespace EtherGizmos.SqlMonitor.Api.Services.Abstractions;

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

    /// <summary>
    /// Adds or updates a record.
    /// </summary>
    /// <param name="record">The record to add or update.</param>
    /// <returns>An awaitable task.</returns>
    void AddOrUpdate(T record);
}
