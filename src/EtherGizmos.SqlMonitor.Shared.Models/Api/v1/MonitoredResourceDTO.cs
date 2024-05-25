using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Extensions;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1;

public class MonitoredResourceDTO
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset? CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

    public Guid? ModifiedByUserId { get; set; }

    [Required]
    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public Task EnsureValid(IQueryable<MonitoredResource> records)
    {
        return Task.CompletedTask;
    }
}

public static class ForMonitoredResourceDTO
{
    public static IProfileExpression AddMonitoredResource(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<MonitoredResource, MonitoredResourceDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        toDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        toDto.MapMember(dest => dest.Name, src => src.Name);
        toDto.MapMember(dest => dest.Description, src => src.Description);
        toDto.MapMember(dest => dest.IsActive, src => src.IsActive);

        var fromDto = @this.CreateMap<MonitoredResourceDTO, MonitoredResource>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        fromDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        fromDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        fromDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        fromDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        fromDto.MapMember(dest => dest.Name, src => src.Name);
        fromDto.MapMember(dest => dest.Description, src => src.Description);
        fromDto.MapMember(dest => dest.IsActive, src => src.IsActive);

        return @this;
    }
}
