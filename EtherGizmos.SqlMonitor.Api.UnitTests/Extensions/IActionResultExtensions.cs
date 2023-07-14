using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using System.Net;

namespace EtherGizmos.SqlMonitor.Api.UnitTests.Extensions;

internal static class IActionResultExtensions
{
    internal static HttpStatusCode? GetStatusCode(this IActionResult @this)
    {
        int? statusCode;
        switch (@this)
        {
            case ObjectResult result:
                statusCode = result.StatusCode;
                break;

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
            case ObjectResult result:
                content = result.Value;
                break;

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

            default:
                throw new InvalidOperationException("Unknown response type: " + @this.GetType());
        }

        return content;
    }
}
