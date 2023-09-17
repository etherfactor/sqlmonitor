namespace EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;

/// <summary>
/// Provides access to internal records.
/// </summary>
/// <typeparam name="T">The type of internal record.</typeparam>
public interface IEditableQueryableService<T> : IQueryableService<T>
    where T : class
{
    /// <summary>
    /// Adds a record to the active result set, if it does not already exist.
    /// </summary>
    /// <param name="record">The record to add.</param>
    void Add(T record);

    /// <summary>
    /// Removes a record from the active result set, if it exists. May delete or only soft-delete a record.
    /// </summary>
    /// <param name="record">The record to remove.</param>
    void Remove(T record);
}
