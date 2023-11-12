using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Api.v1.Enums;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

[Display(Name = "QueryMetricSeverity")]
public class QueryMetricSeverityDTO
{
    [Required]
    [Display(Name = "severity_type")]
    public SeverityTypeDTO? SeverityType { get; set; }

    [Display(Name = "minimum_expression")]
    public string? MinimumExpression { get; set; }

    [Display(Name = "maximum_expression")]
    public string? MaximumExpression { get; set; }
}

public static class ForQueryMetricSeverityDTO
{
    public static IProfileExpression AddQueryMetricSeverity(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<QueryMetricSeverity, QueryMetricSeverityDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.SeverityType, src => src.SeverityType);
        toDto.MapMember(dest => dest.MinimumExpression, src => src.MinimumExpression);
        toDto.MapMember(dest => dest.MaximumExpression, src => src.MaximumExpression);

        var fromDto = @this.CreateMap<QueryMetricSeverityDTO, QueryMetricSeverity>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.SeverityType, src => src.SeverityType);
        fromDto.MapMember(dest => dest.MinimumExpression, src => src.MinimumExpression);
        fromDto.MapMember(dest => dest.MaximumExpression, src => src.MaximumExpression);

        return @this;
    }

    public static ODataModelBuilder AddQueryMetricSeverity(this ODataModelBuilder @this)
    {
        var complex = @this.ComplexType<QueryMetricSeverityDTO>();
        complex.EnumPropertyWithAnnotations(e => e.SeverityType);
        complex.PropertyWithAnnotations(e => e.MinimumExpression);
        complex.PropertyWithAnnotations(e => e.MaximumExpression);

        return @this;
    }
}
