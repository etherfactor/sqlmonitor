using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.SqlMonitor.Shared.Models;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1;
using EtherGizmos.SqlMonitor.Shared.OData.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.SqlMonitor.Shared.OData.Api.v1;

public class ScriptVariantDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var entity = builder.ComplexType<ScriptVariantDTO>();

        entity.Namespace = "EtherGizmos.PerformancePulse";
        entity.Name = entity.Name.Replace("DTO", "");

        entity.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            entity.Property(e => e.ScriptInterpreterId);
            entity.HasRequired(e => e.ScriptInterpreter);
            entity.Property(e => e.ScriptText);
        }
    }
}
