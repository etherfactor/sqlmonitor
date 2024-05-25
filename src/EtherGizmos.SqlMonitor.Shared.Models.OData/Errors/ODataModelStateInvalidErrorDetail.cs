using EtherGizmos.SqlMonitor.Models.OData.Errors.Abstractions;

namespace EtherGizmos.SqlMonitor.Models.OData.Errors;

public class ODataModelStateInvalidErrorDetail : ODataErrorDetailBase
{
    public ODataModelStateInvalidErrorDetail(string code, string propertyName, string message)
        : base(codeProvider: () => code,
            targetProvider: () => propertyName,
            messageProvider: () => message)
    {
    }
}
