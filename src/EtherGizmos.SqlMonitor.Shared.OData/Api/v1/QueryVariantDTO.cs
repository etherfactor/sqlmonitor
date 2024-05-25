using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1;

public class QueryVariantDTO
{
    [Required]
    public SqlType? SqlType { get; set; }

    [Required]
    public string? QueryText { get; set; }
}

public class QueryVariantDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var entity = builder.ComplexType<QueryVariantDTO>();

        entity.Namespace = "EtherGizmos.PerformancePulse";
        entity.Name = entity.Name.Replace("DTO", "");

        entity.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            entity.EnumProperty(e => e.SqlType);
            entity.Property(e => e.QueryText);
        }
    }
}

public static class ForQueryVariantDTO
{
    public static IProfileExpression AddQueryVariant(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<QueryVariant, QueryVariantDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.SqlType, src => src.SqlType);
        toDto.MapMember(dest => dest.QueryText, src => src.QueryText);

        var fromDto = @this.CreateMap<QueryVariantDTO, QueryVariant>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.SqlType, src => src.SqlType);
        fromDto.MapMember(dest => dest.QueryText, src => src.QueryText);

        return @this;
    }
}
