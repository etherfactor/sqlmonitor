using EtherGizmos.SqlMonitor.Shared.OData.Errors.Abstractions;

namespace EtherGizmos.SqlMonitor.Shared.OData.Errors;

public class ODataModelStateInvalidErrorDetail : ODataErrorDetailBase
{
    public ODataModelStateInvalidErrorDetail(string code, string propertyName, string message)
        : base(codeProvider: () => code,
            targetProvider: () => propertyName,
            messageProvider: () => message)
    {
    }
}
