using EtherGizmos.SqlMonitor.Models.Api.v1;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.SqlMonitor.Api.OData.Metadata;

public static class ODataModel
{
    public static IEdmModel GetEdmModel(decimal apiVersion)
    {
        var builder = new ODataModelBuilder();
        builder.Namespace = "SqlMonitor.v1";

        builder.AddInstance();
        builder.AddInstanceQuery();
        builder.AddInstanceQueryDatabase();
        builder.AddPermission();
        builder.AddSecurable();
        builder.AddQuery();
        builder.AddUser();

        var model = builder.GetEdmModel();

        //TODO: Once https://github.com/OData/AspNetCoreOData/issues/887 is resolved, tables should have IAuditableDTO added

        return model;
    }
}
