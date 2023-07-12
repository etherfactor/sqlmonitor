using EtherGizmos.SqlMonitor.Api.OData.Errors.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OData;
using System.Globalization;
using System.Runtime.Serialization;

namespace EtherGizmos.SqlMonitor.Api.Services.Middleware;

public class ReturnODataErrorFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception != null && typeof(ReturnODataErrorException).IsAssignableFrom(context.Exception.GetType()))
        {
            var exception = (ReturnODataErrorException)context.Exception;
            context.Result = exception.Error.GetResponse();

            context.ExceptionHandled = true;
        }
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }
}

public class ReturnODataErrorException : Exception
{
    public ODataErrorBase Error { get; }

    public ReturnODataErrorException(ODataErrorBase error)
    {
        Error = error;
    }

    public ReturnODataErrorException(ODataErrorBase error, string? message) : base(message)
    {
        Error = error;
    }

    public ReturnODataErrorException(ODataErrorBase error, string? message, Exception? innerException) : base(message, innerException)
    {
        Error = error;
    }
}
