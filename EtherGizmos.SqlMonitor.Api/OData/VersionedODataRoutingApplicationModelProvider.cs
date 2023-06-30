using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.OData.Edm;

namespace EtherGizmos.SqlMonitor.Api.OData;

public class VersionedODataRoutingApplicationModelProvider : IApplicationModelProvider
{
    public int Order => 90;

    public void OnProvidersExecuted(ApplicationModelProviderContext context)
    {
        IEdmModel model = EdmCoreModel.Instance;
        string prefix = string.Empty;
        foreach (var controllerModel in context.Result.Controllers)
        {

        }
    }

    public void OnProvidersExecuting(ApplicationModelProviderContext context)
    {
        throw new NotImplementedException();
    }
}
