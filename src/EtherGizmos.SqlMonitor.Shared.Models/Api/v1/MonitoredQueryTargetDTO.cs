using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1.Enums;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Extensions;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1;

public class MonitoredQueryTargetDTO
{
    public int Id { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

    public Guid? ModifiedByUserId { get; set; }

    [Required]
    public Guid? MonitoredSystemId { get; set; }

    public MonitoredSystemDTO? MonitoredSystem { get; set; }

    [Required]
    public Guid? MonitoredResourceId { get; set; }

    public MonitoredResourceDTO? MonitoredResource { get; set; }

    [Required]
    public Guid? MonitoredEnvironmentId { get; set; }

    public MonitoredEnvironmentDTO? MonitoredEnvironment { get; set; }

    [Required]
    public SqlTypeDTO? SqlType { get; set; }

    [Required]
    public string? HostName { get; set; }

    [Required]
    public string? ConnectionString { get; set; }

    public Task EnsureValid(IQueryable<MonitoredQueryTarget> records)
    {
        return Task.CompletedTask;
    }
}

public static class ForMonitoredQueryTargetDTO
{
    public static IProfileExpression AddMonitoredQueryTarget(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<MonitoredQueryTarget, MonitoredQueryTargetDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        toDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        toDto.MapMember(dest => dest.MonitoredSystemId, src => src.MonitoredTarget.MonitoredSystemId);
        toDto.MapMember(dest => dest.MonitoredSystem, src => src.MonitoredTarget.MonitoredSystem);
        toDto.MapMember(dest => dest.MonitoredResourceId, src => src.MonitoredTarget.MonitoredResourceId);
        toDto.MapMember(dest => dest.MonitoredResource, src => src.MonitoredTarget.MonitoredResource);
        toDto.MapMember(dest => dest.MonitoredEnvironmentId, src => src.MonitoredTarget.MonitoredEnvironmentId);
        toDto.MapMember(dest => dest.MonitoredEnvironment, src => src.MonitoredTarget.MonitoredEnvironment);
        toDto.MapMember(dest => dest.SqlType, src => src.SqlType);
        toDto.MapMember(dest => dest.HostName, src => src.HostName);
        toDto.MapMember(dest => dest.ConnectionString, src => src.ConnectionString);

        var fromDto = @this.CreateMap<MonitoredQueryTargetDTO, MonitoredQueryTarget>();
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
        fromDto.MapMember(dest => dest.SqlType, src => src.SqlType);
        fromDto.MapMember(dest => dest.HostName, src => src.HostName);
        fromDto.MapMember(dest => dest.ConnectionString, src => src.ConnectionString);

        return @this;
    }
}
