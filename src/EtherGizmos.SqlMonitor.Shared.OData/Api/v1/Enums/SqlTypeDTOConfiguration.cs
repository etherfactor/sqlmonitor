using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.SqlMonitor.Shared.Models;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1.Enums;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.SqlMonitor.Shared.OData.Api.v1.Enums;

public class SqlTypeDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var enumeration = builder.EnumType<SqlTypeDTO>();

        enumeration.Namespace = "EtherGizmos.PerformancePulse";
        enumeration.Name = enumeration.Name.Replace("DTO", "");

        if (apiVersion >= ApiVersions.V0_1)
        {
        }
    }
}
