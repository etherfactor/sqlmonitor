using EtherGizmos.SqlMonitor.Api.OData.Errors.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Filters;

namespace EtherGizmos.SqlMonitor.Api.Exceptions;

/// <summary>
/// Throw when an OData error needs to be returned to the user, as long as the <see cref="ReturnODataErrorFilter"/> is
/// added to the list of global filters.
/// </summary>
public class ReturnODataErrorException : Exception
{
    /// <summary>
    /// The OData error to return.
    /// </summary>
    public ODataErrorBase Error { get; }

    /// <inheritdoc/>
    /// <param name="error">The OData error to return.</param>
    public ReturnODataErrorException(ODataErrorBase error)
    {
        Error = error;
    }
}
