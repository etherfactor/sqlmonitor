using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using EtherGizmos.SqlMonitor.Shared.Models.Extensions;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1;

public class QueryVariantDTO
{
    [Required]
    public SqlType? SqlType { get; set; }

    [Required]
    public string? QueryText { get; set; }
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
