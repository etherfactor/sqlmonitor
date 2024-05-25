using EtherGizmos.SqlMonitor.Shared.OData.Exceptions;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Shared.OData.Extensions;

public static class IQueryableExtensions
{
    /// <summary>
    /// Ensures that a record with the given properties is unique in a dataset.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="properties">The properties and their values to check for uniqueness. If two different properties must
    /// be independently unique, call this function two separate times.</param>
    /// <exception cref="ReturnODataErrorException"></exception>
    public static void EnsureUnique<TEntity>(this IQueryable<TEntity> @this, params (Expression<Func<TEntity, object?>>, object?)[] properties)
    {
        foreach (var property in properties)
        {
            var propertyRef = property.Item1.Body;
            var parameter = property.Item1.Parameters[0];
            var constantRef = Expression.Constant(property.Item2);
            var comparer = Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(propertyRef, constantRef), parameter);

            @this = @this.Where(comparer);
        }

        if (@this.Any())
        {
            var error = new ODataRecordNotUniqueError<TEntity>(properties);
            throw new ReturnODataErrorException(error);
        }
    }
}
