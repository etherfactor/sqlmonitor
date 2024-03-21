using Asp.Versioning;
using Asp.Versioning.OData;
using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

public class ScriptDTO
{
    public Guid Id { get; set; }

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

    public List<ScriptVariantDTO> Variants { get; set; } = new();

    public Task EnsureValid(IQueryable<Script> records)
    {
        return Task.CompletedTask;
    }
}

public class ScriptDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var entitySet = builder.EntitySet<ScriptDTO>("scripts");
        var entity = builder.EntityType<ScriptDTO>();

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
            entity.Property(e => e.Name);
            entity.Property(e => e.Description);
            entity.Property(e => e.RunFrequency);
            entity.Property(e => e.LastRunAtUtc);
            entity.Property(e => e.IsActive);
        }
    }
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

        return @this;
    }
}
