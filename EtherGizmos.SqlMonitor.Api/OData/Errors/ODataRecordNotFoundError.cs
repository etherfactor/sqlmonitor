using EtherGizmos.SqlMonitor.Api.OData.Errors.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Api.OData.Errors;

/// <summary>
/// Utilize when a record was expected but could not be found.
/// </summary>
/// <typeparam name="T">The externally-facing type of record.</typeparam>
public class ODataRecordNotFoundError<T> : ODataErrorBase
{
    private const string Code = "test-0001";

    /// <summary>
    /// Construct the error.
    /// </summary>
    /// <param name="keys">A set of key properties and their values that failed to find a record.</param>
    public ODataRecordNotFoundError(params (Expression<Func<T, object?>>, object)[] keys)
        : base(codeProvider: () => Code,
            targetProvider: null,
            messageProvider: () => "A record with the specified key(s)/value(s) was not found.")
    {
        foreach (var key in keys)
        {
            var property = key.Item1;
            var value = key.Item2;

            var detail = new ODataRecordNotFoundErrorDetail<T>(Code, property, value);
            AddDetail(detail);
        }
    }

    /// <inheritdoc/>
    public override IActionResult GetResponse()
    {
        return new NotFoundODataResult(GetError());
    }
}
