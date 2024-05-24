namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface ICanCompare<TEntity, TProperty>
{
    /// <summary>
    /// Filters to entities equal to the value specified.
    /// </summary>
    /// <param name="value">The value to match.</param>
    /// <returns>The filtered entity set.</returns>
    ICacheFiltered<TEntity> IsEqualTo(TProperty value);

    /// <summary>
    /// Filters to entities greater than the value specified.
    /// </summary>
    /// <param name="value">The value to match.</param>
    /// <returns>The filtered entity set.</returns>
    ICacheFiltered<TEntity> IsGreaterThan(TProperty value);

    /// <summary>
    /// Filters to entities greater than or equal to the value specified.
    /// </summary>
    /// <param name="value">The value to match.</param>
    /// <returns>The filtered entity set.</returns>
    ICacheFiltered<TEntity> IsGreaterThanOrEqualTo(TProperty value);

    /// <summary>
    /// Filters to entities less than the value specified.
    /// </summary>
    /// <param name="value">The value to match.</param>
    /// <returns>The filtered entity set.</returns>
    ICacheFiltered<TEntity> IsLessThan(TProperty value);

    /// <summary>
    /// Filters to entities less than or equal to the value specified.
    /// </summary>
    /// <param name="value">The value to match.</param>
    /// <returns>The filtered entity set.</returns>
    ICacheFiltered<TEntity> IsLessThanOrEqualTo(TProperty value);

    /// <summary>
    /// Filteres to entities between the specified values, inclusive.
    /// </summary>
    /// <param name="valueStart">The starting value to match.</param>
    /// <param name="valueEnd">The ending value to match.</param>
    /// <returns>The filtered entity set.</returns>
    ICacheFiltered<TEntity> IsBetween(TProperty valueStart, TProperty valueEnd);
}
