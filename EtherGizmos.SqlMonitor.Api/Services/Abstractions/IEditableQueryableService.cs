namespace EtherGizmos.SqlMonitor.Api.Services.Abstractions;

/// <summary>
/// Provides access to internal records.
/// </summary>
/// <typeparam name="T">The type of internal record.</typeparam>
public interface IEditableQueryableService<T> : IQueryableService<T>
    where T : class
{
    /// <summary>
    /// Adds a record, if it does not already exist.
    /// </summary>
    /// <param name="record">The record to add.</param>
    void Add(T record);

    /// <summary>
    /// Removes a record, if it exists.
    /// </summary>
    /// <param name="record">The record to remove.</param>
    void Remove(T record);
}
