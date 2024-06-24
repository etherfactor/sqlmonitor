using EtherGizmos.SqlMonitor.Models.OData.Errors.Abstractions;

namespace EtherGizmos.SqlMonitor.Models.Exceptions;

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
