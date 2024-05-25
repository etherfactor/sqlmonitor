using EtherGizmos.SqlMonitor.Shared.OData.Errors.Abstractions;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Shared.OData.Errors;

/// <summary>
/// Utilize when a record references the same entity more than once, in a way that is not allowed.
/// </summary>
/// <typeparam name="TEntity">The externally-facing type of record.</typeparam>
public class ODataDuplicateReferenceError<TEntity> : ODataErrorBase
{
    private const string Code = "test-0000";

    public ODataDuplicateReferenceError(params (bool First, Expression<Func<TEntity, object?>> Expression)[] duplicatePaths)
        : base(() => Code,
            null,
            () => "Multiple references to the same entity were provided, but only one is allowed.")
    {
        foreach (var duplicatePath in duplicatePaths)
        {
            var detail = new ODataDuplicateReferenceErrorDetail<TEntity>(Code, duplicatePath.First, duplicatePath.Expression);
            AddDetail(detail);
        }
    }
}
