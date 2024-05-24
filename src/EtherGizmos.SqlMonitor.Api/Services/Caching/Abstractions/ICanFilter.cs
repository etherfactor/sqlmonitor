using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;

public interface ICanFilter<TEntity>
{
    /// <summary>
    /// Applies a filter to the entity set, limiting the number of returned records to those matching the specified
    /// conditions.
    /// </summary>
    /// <typeparam name="TProperty">The type of property on which the set is being filtered.</typeparam>
    /// <param name="indexedProperty">
    ///     The indexed property on which the set is being filtered. The selected property of
    ///     <typeparamref name="TEntity"/> *must* be annotated with an <see cref="IndexedAttribute"/>.
    /// </param>
    /// <returns></returns>
    ICanCompare<TEntity, TProperty> Where<TProperty>(Expression<Func<TEntity, TProperty>> indexedProperty);
}
