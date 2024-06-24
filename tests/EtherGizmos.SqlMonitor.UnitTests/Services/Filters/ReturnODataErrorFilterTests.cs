using EtherGizmos.SqlMonitor.Api.Core.Services.Filters;
using EtherGizmos.SqlMonitor.Shared.OData.Errors;
using EtherGizmos.SqlMonitor.Shared.OData.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace EtherGizmos.SqlMonitor.UnitTests.Services.Filters;

internal class ReturnODataErrorFilterTests
{
    private ReturnODataErrorFilter Filter { get; set; }

    [SetUp]
    public void SetUp()
    {
        Filter = new ReturnODataErrorFilter();
    }

    [Test]
    public void OnActionExecuted_NoException_DoesNotHandle()
    {
        var actionContext = new ActionContext(
            Mock.Of<HttpContext>(),
            Mock.Of<RouteData>(),
            Mock.Of<ActionDescriptor>(),
            Mock.Of<ModelStateDictionary>());

        var executedContext = new ActionExecutedContext(
            actionContext,
            new List<IFilterMetadata>(),
            Mock.Of<Controller>())
        {
            Exception = null,
            ExceptionHandled = false,
            Result = null
        };

        Filter.OnActionExecuted(executedContext);

        Assert.Multiple(() =>
        {
            Assert.That(executedContext.Exception, Is.Null);
            Assert.That(executedContext.ExceptionHandled, Is.False);
            Assert.That(executedContext.Result, Is.Null);
        });
    }

    [Test]
    public void OnActionExecuted_WithException_DoesHandle()
    {
        var error = new ODataParameterNotApplicableOnSingleError("$filter");
        var exception = new ReturnODataErrorException(error);

        var actionContext = new ActionContext(
            Mock.Of<HttpContext>(),
            Mock.Of<RouteData>(),
            Mock.Of<ActionDescriptor>(),
            Mock.Of<ModelStateDictionary>());

        var executedContext = new ActionExecutedContext(
            actionContext,
            new List<IFilterMetadata>(),
            Mock.Of<Controller>())
        {
            Exception = exception,
            ExceptionHandled = false,
            Result = null
        };

        Filter.OnActionExecuted(executedContext);

        Assert.Multiple(() =>
        {
            Assert.That(executedContext.Exception, Is.EqualTo(exception));
            Assert.That(executedContext.ExceptionHandled, Is.True);
            Assert.That(executedContext.Result, Is.Not.Null);
        });
    }
}
