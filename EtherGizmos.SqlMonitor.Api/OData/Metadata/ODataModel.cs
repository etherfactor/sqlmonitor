using EtherGizmos.SqlMonitor.Models.Api.v1;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.SqlMonitor.Api.OData.Metadata;

public static class ODataModel
{
    public static IEdmModel GetEdmModel(decimal apiVersion)
    {
        var builder = new ODataModelBuilder();

        builder.AddSecurable();

        var model = builder.GetEdmModel();

        //TODO: Once https://github.com/OData/AspNetCoreOData/issues/887 is resolved, tables should have IAuditableDTO added

        return model;
    }
}
