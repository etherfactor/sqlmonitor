using EtherGizmos.SqlMonitor.Models.Extensions;
using EtherGizmos.SqlMonitor.Models.OData.Errors.Abstractions;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Models.OData.Errors;

/// <summary>
/// Utilize when a record references the same entity more than once, in a way that is not allowed.
/// </summary>
/// <typeparam name="TEntity">The externally-facing type of record.</typeparam>
public class ODataDuplicateReferenceErrorDetail<TEntity> : ODataErrorDetailBase
{
    public ODataDuplicateReferenceErrorDetail(string code, bool isFirstAndThusNotDuplicate, Expression<Func<TEntity, object?>> duplicatePath)
        : base(() => code,
            () => GetExpressionPath(duplicatePath),
            () => isFirstAndThusNotDuplicate
                ? "First instance of this reference."
                : "Duplicate reference of a previously-found reference.")
    {
        GetExpressionPath(duplicatePath);
    }

    private static string GetExpressionPath(Expression<Func<TEntity, object?>> expression)
    {
        var str = expression.GetPath();
        return str;
    }
}
