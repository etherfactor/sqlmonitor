using EtherGizmos.SqlMonitor.Shared.OData.Errors.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Shared.OData.Errors;

public class ODataRecordNotUniqueErrorDetail<TEntity> : ODataErrorDetailBase
{
    public ODataRecordNotUniqueErrorDetail(string code, Expression<Func<TEntity, object?>> selector, object? value)
        : base(codeProvider: () => code,
            targetProvider: () => selector.GetPropertyInfo().GetCustomAttribute<DisplayAttribute>()?.Name
                ?? throw new InvalidOperationException(string.Format("The type {0} must be annotated with a {1}.", typeof(TEntity), nameof(DisplayAttribute))),
            messageProvider: () => string.Format("Duplicated unique value: {0}", value))
    {
    }
}
