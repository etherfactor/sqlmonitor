using Asp.Versioning;
using Asp.Versioning.OData;
using EtherGizmos.SqlMonitor.Shared.Models;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1;
using EtherGizmos.SqlMonitor.Shared.OData.Extensions;
using Microsoft.OData.ModelBuilder;

namespace EtherGizmos.SqlMonitor.Shared.OData.Api.v1;

public class MonitoredQueryTargetDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var entitySet = builder.EntitySet<MonitoredQueryTargetDTO>("monitoredQueryTargets");
        var entity = builder.EntityType<MonitoredQueryTargetDTO>();

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
            entity.EnumProperty(e => e.SqlType);
            entity.Property(e => e.HostName);
            entity.Property(e => e.ConnectionString);
        }
    }
}
