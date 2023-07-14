using EtherGizmos.SqlMonitor.Api.OData.Errors.Abstractions;
using EtherGizmos.SqlMonitor.Models.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.OData.Errors;

/// <summary>
/// Utilize when a record was expected but none could be found.
/// </summary>
/// <typeparam name="T">The externally-facing type of record.</typeparam>
public class ODataRecordNotFoundErrorDetail<T> : ODataErrorDetailBase
{
    /// <summary>
    /// Construct the error detail.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="selector">A single key.</param>
    /// <param name="value">The value of that key that failed to find a record.</param>
    public ODataRecordNotFoundErrorDetail(string code, Expression<Func<T, object?>> selector, object? value)
        : base(codeProvider: () => code,
            targetProvider: () => selector.GetPropertyInfo().GetCustomAttribute<DisplayAttribute>()?.Name
                ?? throw new InvalidOperationException(string.Format("The type {0} must be annotated with a {1}.", typeof(T), nameof(DisplayAttribute))),
            messageProvider: () => string.Format("Non-existent key value: {0}", value))
    {
    }
}
