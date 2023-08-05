using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

[Display(Name = "InstanceQueryDatabase")]
public class InstanceQueryDatabaseDTO
{
    [Required]
    [Display(Name = "query_id")]
    public Guid? QueryId { get; set; }

    [Display(Name = "query")]
    public QueryDTO? Query { get; set; }

    [Required]
    [Display(Name = "database")]
    public string? Database { get; set; }
}

public static class ForInstanceQueryDatabaseDTO
{
    public static IProfileExpression AddInstanceQueryDatabase(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<InstanceQueryDatabase, InstanceQueryDatabaseDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.QueryId, src => src.QueryId);
        toDto.MapMember(dest => dest.Query, src => src.Query);
        toDto.MapMember(dest => dest.Database, src => src.DatabaseOverride);

        var fromDto = @this.CreateMap<InstanceQueryDatabaseDTO, InstanceQueryDatabase>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.QueryId, src => src.QueryId);
        fromDto.MapMember(dest => dest.Query, src => src.Query);
        fromDto.MapMember(dest => dest.DatabaseOverride, src => src.Database);

        return @this;
    }

    public static ODataModelBuilder AddInstanceQueryDatabase(this ODataModelBuilder @this)
    {
        var complex = @this.ComplexTypeWithAnnotations<InstanceQueryDatabaseDTO>();
        complex.PropertyWithAnnotations(e => e.QueryId);
        complex.HasRequiredWithAnnotations(e => e.Query);
        complex.PropertyWithAnnotations(e => e.Database);

        return @this;
    }
}
