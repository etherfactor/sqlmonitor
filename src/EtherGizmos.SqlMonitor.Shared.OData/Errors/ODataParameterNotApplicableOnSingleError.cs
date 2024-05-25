using EtherGizmos.SqlMonitor.Shared.OData.Errors.Abstractions;

namespace EtherGizmos.SqlMonitor.Shared.OData.Errors;

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
