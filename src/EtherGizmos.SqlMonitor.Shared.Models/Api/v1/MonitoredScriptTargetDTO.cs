using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1.Enums;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Extensions;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1;

public class MonitoredScriptTargetDTO
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
    public int? ScriptInterpreterId { get; set; }

    public ScriptInterpreterDTO? ScriptInterpreter { get; set; }

    [Required]
    public ExecTypeDTO? ExecType { get; set; }

    [Required]
    public string? HostName { get; set; }

    public int? Port { get; set; }

    [Required]
    public string? FilePath { get; set; }

    public bool? UseSsl { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public Task EnsureValid(IQueryable<MonitoredScriptTarget> records)
    {
        return Task.CompletedTask;
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
        toDto.MapMember(dest => dest.MonitoredSystem, src => src.MonitoredTarget.MonitoredSystem);
        toDto.MapMember(dest => dest.MonitoredResourceId, src => src.MonitoredTarget.MonitoredResourceId);
        toDto.MapMember(dest => dest.MonitoredResource, src => src.MonitoredTarget.MonitoredResource);
        toDto.MapMember(dest => dest.MonitoredEnvironmentId, src => src.MonitoredTarget.MonitoredEnvironmentId);
        toDto.MapMember(dest => dest.MonitoredEnvironment, src => src.MonitoredTarget.MonitoredEnvironment);
        toDto.MapMember(dest => dest.ScriptInterpreterId, src => src.ScriptInterpreterId);
        toDto.MapMember(dest => dest.ScriptInterpreter, src => src.ScriptInterpreter);
        toDto.MapMember(dest => dest.ExecType, src => src.ExecType);
        toDto.MapMember(dest => dest.HostName, src => src.HostName);
        toDto.MapMember(dest => dest.Port, src => src.Port);
        toDto.MapMember(dest => dest.FilePath, src => src.RunInPath);
        toDto.MapMember(dest => dest.Username, src => src.SshUsername);
        toDto.MapMember(dest => dest.Password, src => src.SshPassword);

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
        fromDto.MapMember(dest => dest.ExecType, src => src.ExecType);
        fromDto.MapMember(dest => dest.HostName, src => src.HostName);
        fromDto.MapMember(dest => dest.Port, src => src.Port);
        fromDto.MapMember(dest => dest.RunInPath, src => src.FilePath);
        fromDto.MapMember(dest => dest.SshUsername, src => src.Username);
        fromDto.MapMember(dest => dest.SshPassword, src => src.Password);

        return @this;
    }
}
