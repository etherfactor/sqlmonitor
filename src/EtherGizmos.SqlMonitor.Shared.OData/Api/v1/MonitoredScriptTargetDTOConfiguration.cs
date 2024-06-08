using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.SqlMonitor.Shared.Models;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1;
using EtherGizmos.SqlMonitor.Shared.OData.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.SqlMonitor.Shared.OData.Api.v1;

public class MonitoredScriptTargetDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var entitySet = builder.EntitySet<MonitoredScriptTargetDTO>("monitoredScriptTargets");
        var entity = builder.EntityType<MonitoredScriptTargetDTO>();

        entity.Namespace = "EtherGizmos.PerformancePulse";
        entity.Name = entity.Name.Replace("DTO", "");

        entity.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id);
            /* Begin Audit */
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.CreatedByUserId);
            entity.Property(e => e.ModifiedAt);
            entity.Property(e => e.ModifiedByUserId);
            /*  End Audit  */
            entity.Property(e => e.MonitoredSystemId);
            entity.HasRequired(e => e.MonitoredSystem);
            entity.Property(e => e.MonitoredResourceId);
            entity.HasRequired(e => e.MonitoredResource);
            entity.Property(e => e.MonitoredEnvironmentId);
            entity.HasRequired(e => e.MonitoredEnvironment);
            entity.Property(e => e.ScriptInterpreterId);
            entity.HasRequired(e => e.ScriptInterpreter);
            entity.EnumProperty(e => e.ExecType);
            entity.Property(e => e.HostName);
            entity.Property(e => e.Port);
            entity.Property(e => e.FilePath);
            entity.Property(e => e.Username);
            entity.Property(e => e.Password);
        }
    }
}
