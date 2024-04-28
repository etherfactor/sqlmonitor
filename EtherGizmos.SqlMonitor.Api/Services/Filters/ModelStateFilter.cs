using EtherGizmos.SqlMonitor.Models.Exceptions;
using EtherGizmos.SqlMonitor.Models.OData.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OData.Extensions;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Services.Filters;

/// <summary>
/// Catches model validation errors before the controller is executed.
/// </summary>
public class ModelStateFilter : IActionFilter
{
    /// <inheritdoc/>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            //Find the first argument marked with [FromBody]
            var argumentType = context.ActionDescriptor.Parameters.OfType<ControllerParameterDescriptor>()
                .FirstOrDefault(e => e.ParameterInfo.GetCustomAttribute<FromBodyAttribute>() != null)
                ?.ParameterType;

            var oDataFeature = context.HttpContext.ODataFeature();

            //Throw an error for the type of that argument
            if (argumentType != null)
            {
                var error = new ODataModelStateInvalidError(oDataFeature.Model, argumentType, context.ModelState);
                throw new ReturnODataErrorException(error);
            }
        }
    }

    /// <inheritdoc/>
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
