using EtherGizmos.SqlMonitor.Shared.OData.Errors.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Shared.OData.Errors;

/// <summary>
/// Utilize when a record was expected but none could be found.
/// </summary>
/// <typeparam name="TEntity">The externally-facing type of record.</typeparam>
public class ODataRecordNotFoundError<TEntity> : ODataErrorBase
{
    private const string Code = "test-0000";

    /// <summary>
    /// Construct the error.
    /// </summary>
    /// <param name="keys">A set of key properties and their values that failed to find a record.</param>
    public ODataRecordNotFoundError(params (Expression<Func<TEntity, object?>>, object)[] keys)
        : base(codeProvider: () => Code,
            targetProvider: null,
            messageProvider: () => "A record with the specified key(s)/value(s) was not found.")
    {
        foreach (var key in keys)
        {
            var property = key.Item1;
            var value = key.Item2;

            var detail = new ODataRecordNotFoundErrorDetail<TEntity>(Code, property, value);
            AddDetail(detail);
        }
    }

    /// <inheritdoc/>
    public override IActionResult GetResponse()
    {
        return new NotFoundODataResult(GetError());
    }
}
