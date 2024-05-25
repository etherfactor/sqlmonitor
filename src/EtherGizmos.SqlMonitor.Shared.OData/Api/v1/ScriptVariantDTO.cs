using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1;

public class ScriptVariantDTO
{
    [Required]
    public int? ScriptInterpreterId { get; set; }

    public ScriptInterpreterDTO? ScriptInterpreter { get; set; }

    [Required]
    public string? ScriptText { get; set; }
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

        var fromDto = @this.CreateMap<ScriptVariantDTO, ScriptVariant>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.ScriptInterpreterId, src => src.ScriptInterpreterId);
        fromDto.MapMember(dest => dest.ScriptText, src => src.ScriptText);

        return @this;
    }
}
