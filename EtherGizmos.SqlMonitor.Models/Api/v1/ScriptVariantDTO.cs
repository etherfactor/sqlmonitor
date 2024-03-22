using Asp.Versioning;
using Asp.Versioning.OData;
using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

public class ScriptVariantDTO
{
    [Required]
    public int? ScriptInterpreterId { get; set; }

    public ScriptInterpreterDTO? ScriptInterpreter { get; set; }

    [Required]
    public string? ScriptText { get; set; }

    public string? TimestampKey { get; set; }

    public string? BucketKey { get; set; }
}

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
            entity.Property(e => e.TimestampKey);
            entity.Property(e => e.BucketKey);
        }
    }
}

public static class ForScriptVariantDTO
{
    public static IProfileExpression AddScriptVariant(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<ScriptVariant, ScriptVariantDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.ScriptInterpreterId, src => src.ScriptInterpreterId);
        toDto.MapMember(dest => dest.ScriptInterpreter, src => src.ScriptInterpreter);
        toDto.MapMember(dest => dest.ScriptText, src => src.ScriptText);
        toDto.MapMember(dest => dest.TimestampKey, src => src.TimestampKey);
        toDto.MapMember(dest => dest.BucketKey, src => src.BucketKey);

        var fromDto = @this.CreateMap<ScriptVariantDTO, ScriptVariant>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.ScriptInterpreterId, src => src.ScriptInterpreterId);
        fromDto.MapMember(dest => dest.ScriptText, src => src.ScriptText);
        fromDto.MapMember(dest => dest.TimestampKey, src => src.TimestampKey);
        fromDto.MapMember(dest => dest.BucketKey, src => src.BucketKey);

        return @this;
    }
}
