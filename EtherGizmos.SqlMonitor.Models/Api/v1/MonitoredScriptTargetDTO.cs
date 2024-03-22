using Asp.Versioning;
using Asp.Versioning.OData;
using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

public class MonitoredScriptTargetDTO
{
    public int Id { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

    public Guid? ModifiedByUserId { get; set; }

    [Required]
    public Guid? MonitoredSystemId { get; set; }

    [Required]
    public Guid? MonitoredResourceId { get; set; }

    [Required]
    public Guid? MonitoredEnvironmentId { get; set; }

    [Required]
    public int? ScriptInterpreterId { get; set; }

    [Required]
    public string? HostName { get; set; }

    [Required]
    public string? FilePath { get; set; }

    public Task EnsureValid(IQueryable<MonitoredScriptTarget> records)
    {
        return Task.CompletedTask;
    }
}

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
            entity.Property(e => e.MonitoredResourceId);
            entity.Property(e => e.MonitoredEnvironmentId);
            entity.Property(e => e.ScriptInterpreterId);
            entity.Property(e => e.HostName);
            entity.Property(e => e.FilePath);
        }
    }
}

public static class ForMonitoredScriptTargetDTO
{
    public static IProfileExpression AddMonitoredScriptTarget(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<MonitoredScriptTarget, MonitoredScriptTargetDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        toDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        toDto.MapMember(dest => dest.MonitoredSystemId, src => src.MonitoredTarget.MonitoredSystemId);
        toDto.MapMember(dest => dest.MonitoredResourceId, src => src.MonitoredTarget.MonitoredResourceId);
        toDto.MapMember(dest => dest.MonitoredEnvironmentId, src => src.MonitoredTarget.MonitoredEnvironmentId);
        toDto.MapMember(dest => dest.ScriptInterpreterId, src => src.ScriptInterpreterId);
        toDto.MapMember(dest => dest.HostName, src => src.HostName);
        toDto.MapMember(dest => dest.FilePath, src => src.FilePath);

        var fromDto = @this.CreateMap<MonitoredScriptTargetDTO, MonitoredScriptTarget>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        fromDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        fromDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        fromDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        fromDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        fromDto.MapPath(dest => dest.MonitoredTarget.MonitoredSystemId, src => src.MonitoredSystemId);
        fromDto.MapPath(dest => dest.MonitoredTarget.MonitoredResourceId, src => src.MonitoredResourceId);
        fromDto.MapPath(dest => dest.MonitoredTarget.MonitoredEnvironmentId, src => src.MonitoredEnvironmentId);
        fromDto.MapMember(dest => dest.ScriptInterpreterId, src => src.ScriptInterpreterId);
        fromDto.MapMember(dest => dest.HostName, src => src.HostName);
        fromDto.MapMember(dest => dest.FilePath, src => src.FilePath);

        return @this;
    }
}
