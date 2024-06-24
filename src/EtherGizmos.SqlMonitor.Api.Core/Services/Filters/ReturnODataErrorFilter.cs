using EtherGizmos.SqlMonitor.Shared.OData.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EtherGizmos.SqlMonitor.Api.Core.Services.Filters;

/// <summary>
/// Catches <see cref="ReturnODataErrorException"/> and returns the inner error in the response.
/// </summary>
public class ReturnODataErrorFilter : IActionFilter
{
    /// <inheritdoc/>
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    /// <inheritdoc/>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception != null && typeof(ReturnODataErrorException).IsAssignableFrom(context.Exception.GetType()))
        {
            var exception = (ReturnODataErrorException)context.Exception;
            context.Result = exception.Error.GetResponse();

            //Mark the exception as handled so it's no longer considered an exception
            context.ExceptionHandled = true;
        }
    }
}
