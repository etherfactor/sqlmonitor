using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Extensions;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1;

public class ScriptDTO
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset? CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

    public Guid? ModifiedByUserId { get; set; }

    [Required]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [Required]
    public TimeSpan? RunFrequency { get; set; }

    public DateTimeOffset? LastRunAtUtc { get; set; }

    public bool? IsActive { get; set; }

    public string? BucketKey { get; set; }

    public string? TimestampUtcKey { get; set; }

    public List<ScriptVariantDTO> Variants { get; set; } = new();

    public List<ScriptMetricDTO> Metrics { get; set; } = new();
}

public static class ForScriptDTO
{
    public static IProfileExpression AddScript(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<Script, ScriptDTO>();
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
        toDto.MapMember(dest => dest.RunFrequency, src => src.RunFrequency);
        toDto.MapMember(dest => dest.LastRunAtUtc, src => src.LastRunAtUtc);
        toDto.MapMember(dest => dest.IsActive, src => src.IsActive);
        toDto.MapMember(dest => dest.BucketKey, src => src.BucketKey);
        toDto.MapMember(dest => dest.TimestampUtcKey, src => src.TimestampUtcKey);
        toDto.MapMember(dest => dest.Variants, src => src.Variants);
        toDto.MapMember(dest => dest.Metrics, src => src.Metrics);

        var fromDto = @this.CreateMap<ScriptDTO, Script>();
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
        fromDto.MapMember(dest => dest.RunFrequency, src => src.RunFrequency);
        fromDto.MapMember(dest => dest.IsActive, src => src.IsActive);
        fromDto.MapMember(dest => dest.BucketKey, src => src.BucketKey);
        fromDto.MapMember(dest => dest.TimestampUtcKey, src => src.TimestampUtcKey);
        fromDto.MapMember(dest => dest.Variants, src => src.Variants);
        fromDto.MapMember(dest => dest.Metrics, src => src.Metrics);

        return @this;
    }
}
