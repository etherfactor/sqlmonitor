﻿using EtherGizmos.SqlMonitor.Api.OData.Errors.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.OData.Errors;

public class ODataParameterDisallowedOnSingleError : ODataErrorBase
{
    public ODataParameterDisallowedOnSingleError(string parameter)
        : base(codeProvider: () => "test-0000",
            targetProvider: () => parameter,
            messageProvider: () => "The provided query parameter is not applicable for single records.")
    {
    }
}
