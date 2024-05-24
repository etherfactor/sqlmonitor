using EtherGizmos.SqlMonitor.Models.OData.Errors.Abstractions;

namespace EtherGizmos.SqlMonitor.Models.OData.Errors;

public class ODataParameterNotApplicableOnSingleError : ODataErrorBase
{
    private const string Code = "test-0000";

    public ODataParameterNotApplicableOnSingleError(string parameter)
        : base(codeProvider: () => Code,
            targetProvider: () => parameter,
            messageProvider: () => "The provided query parameter is not applicable for single records.")
    {
    }
}
