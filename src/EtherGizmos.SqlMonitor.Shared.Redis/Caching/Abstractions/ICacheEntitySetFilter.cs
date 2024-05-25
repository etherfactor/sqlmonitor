using System.Reflection;

namespace EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;

public interface ICacheEntitySetFilter<TEntity>
{
    /// <summary>
    /// Get whether the end score is included in the range.
    /// </summary>
    /// <returns>The end score inclusivity.</returns>
    bool GetEndInclusivity();

    /// <summary>
    /// Get the end score of the filter.
    /// </summary>
    /// <returns>The end score.</returns>
    double GetEndScore();

    /// <summary>
    /// Get the indexed property of the filter.
    /// </summary>
    /// <returns>The indexed property.</returns>
    PropertyInfo GetProperty();

    /// <summary>
    /// Get whether the start score is included in the range.
    /// </summary>
    /// <returns>The start score inclusivity.</returns>
    bool GetStartInclusivity();

    /// <summary>
    /// Get the start score of the filter.
    /// </summary>
    /// <returns>The start score.</returns>
    double GetStartScore();
}
