using System.Net;

namespace EtherGizmos.SqlMonitor.UnitTests.Extensions;

internal static class IActionResultExtensions
{
    internal static HttpStatusCode? GetStatusCode(this IActionResult @this)
    {
        int? statusCode;
        switch (@this)
        {
            case BadRequestODataResult result:
                statusCode = result.StatusCode;
                break;

            case BadRequestResult result:
                statusCode = result.StatusCode;
                break;

            case ConflictODataResult result:
                statusCode = result.StatusCode;
                break;

            case ConflictResult result:
                statusCode = result.StatusCode;
                break;

            case NoContentResult result:
                statusCode = result.StatusCode;
                break;

            case NotFoundODataResult result:
                statusCode = result.StatusCode;
                break;

            case NotFoundResult result:
                statusCode = result.StatusCode;
                break;

            case OkResult result:
                statusCode = result.StatusCode;
                break;

            case UnauthorizedODataResult result:
                statusCode = result.StatusCode;
                break;

            case UnauthorizedResult result:
                statusCode = result.StatusCode;
                break;

            case UnprocessableEntityODataResult result:
                statusCode = result.StatusCode;
                break;

            case UnprocessableEntityResult result:
                statusCode = result.StatusCode;
                break;

            case ObjectResult result when @this.GetType().IsGenericType && @this.GetType().GetGenericTypeDefinition() == typeof(CreatedODataResult<>):
                statusCode = (int)HttpStatusCode.Created;
                break;

            case ObjectResult result:
                statusCode = result.StatusCode;
                break;

            default:
                throw new InvalidOperationException("Unknown response type: " + @this.GetType());
        }

        return (HttpStatusCode?)statusCode;
    }

    internal static object? GetContent(this IActionResult @this)
    {
        object? content;
        switch (@this)
        {
            case BadRequestODataResult result:
                content = result.Error;
                break;

            case BadRequestResult result:
                content = null;
                break;

            case ConflictODataResult result:
                content = result.Error;
                break;

            case ConflictResult result:
                content = null;
                break;

            case NoContentResult result:
                content = null;
                break;

            case NotFoundODataResult result:
                content = result.Error;
                break;

            case NotFoundResult result:
                content = null;
                break;

            case OkResult result:
                content = null;
                break;

            case UnauthorizedODataResult result:
                content = result.Error;
                break;

            case UnauthorizedResult result:
                content = null;
                break;

            case UnprocessableEntityODataResult result:
                content = result.Error;
                break;

            case UnprocessableEntityResult result:
                content = null;
                break;

            case ObjectResult result when @this.GetType().IsGenericType && @this.GetType().GetGenericTypeDefinition() == typeof(CreatedODataResult<>):
                content = result.Value;
                break;

            case ObjectResult result:
                content = result.Value;
                break;

            default:
                throw new InvalidOperationException("Unknown response type: " + @this.GetType());
        }

        return content;
    }
}
