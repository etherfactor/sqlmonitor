using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

[Display(Name = "InstanceQuery")]
public class InstanceQueryDTO
{
    [Required]
    [Display(Name = "query_id")]
    public Guid? QueryId { get; set; }

    [Display(Name = "query")]
    public QueryDTO? Query { get; set; }
}

public static class ForInstanceQueryDTO
{
    public static IProfileExpression AddInstanceQuery(this IProfileExpression @this)
    {
        var toDtoB = @this.CreateMap<QueryInstanceBlacklist, InstanceQueryDTO>();
        toDtoB.IgnoreAllMembers();
        toDtoB.MapMember(dest => dest.QueryId, src => src.QueryId);
        toDtoB.MapMember(dest => dest.Query, src => src.Query);

        var fromDtoB = @this.CreateMap<InstanceQueryDTO, QueryInstanceBlacklist>();
        fromDtoB.IgnoreAllMembers();
        fromDtoB.MapMember(dest => dest.QueryId, src => src.QueryId);
        fromDtoB.MapMember(dest => dest.Query, src => src.Query);

        var toDtoW = @this.CreateMap<QueryInstanceWhitelist, InstanceQueryDTO>();
        toDtoW.IgnoreAllMembers();
        toDtoW.MapMember(dest => dest.QueryId, src => src.QueryId);
        toDtoW.MapMember(dest => dest.Query, src => src.Query);

        var fromDtoW = @this.CreateMap<InstanceQueryDTO, QueryInstanceWhitelist>();
        fromDtoW.IgnoreAllMembers();
        fromDtoW.MapMember(dest => dest.QueryId, src => src.QueryId);
        fromDtoW.MapMember(dest => dest.Query, src => src.Query);

        return @this;
    }

    public static ODataModelBuilder AddInstanceQuery(this ODataModelBuilder @this)
    {
        var complex = @this.ComplexTypeWithAnnotations<InstanceQueryDTO>();
        complex.PropertyWithAnnotations(e => e.QueryId);
        complex.HasRequiredWithAnnotations(e => e.Query);

        return @this;
    }
}
