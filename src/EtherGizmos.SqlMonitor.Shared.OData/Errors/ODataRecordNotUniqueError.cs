using EtherGizmos.SqlMonitor.Shared.OData.Errors.Abstractions;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Shared.OData.Errors;

public class ODataRecordNotUniqueError<TEntity> : ODataErrorBase
{
    private const string Code = "test-0000";

    public ODataRecordNotUniqueError((Expression<Func<TEntity, object?>>, object?)[] keys)
        : base(codeProvider: () => Code,
            targetProvider: () => null,
            messageProvider: () => "The specified key(s)/value(s) conflict with an existing record.")
    {
        foreach (var key in keys)
        {
            var property = key.Item1;
            var value = key.Item2;

            var detail = new ODataRecordNotUniqueErrorDetail<TEntity>(Code, property, value);
            AddDetail(detail);
        }
    }
}
