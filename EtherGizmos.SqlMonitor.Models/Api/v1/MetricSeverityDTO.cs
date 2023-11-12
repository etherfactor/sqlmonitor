using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Api.v1.Enums;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

[Display(Name = "MetricSeverity")]
public class MetricSeverityDTO
{
    [Required]
    [Display(Name = "severity_type")]
    public SeverityTypeDTO? SeverityType { get; set; }

    [Display(Name = "minimum_value")]
    public double? MinimumValue { get; set; }

    [Display(Name = "maximum_value")]
    public double? MaximumValue { get; set; }
}

public static class ForMetricSeverityDTO
{
    public static IProfileExpression AddMetricSeverity(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<MetricSeverity, MetricSeverityDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.SeverityType, src => src.SeverityType);
        toDto.MapMember(dest => dest.MinimumValue, src => src.MinimumValue);
        toDto.MapMember(dest => dest.MaximumValue, src => src.MaximumValue);

        var fromDto = @this.CreateMap<MetricSeverityDTO, MetricSeverity>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.SeverityType, src => src.SeverityType);
        fromDto.MapMember(dest => dest.MinimumValue, src => src.MinimumValue);
        fromDto.MapMember(dest => dest.MaximumValue, src => src.MaximumValue);

        return @this;
    }

    public static ODataModelBuilder AddMetricSeverity(this ODataModelBuilder @this)
    {
        var complex = @this.ComplexTypeWithAnnotations<MetricSeverityDTO>();
        complex.EnumPropertyWithAnnotations(e => e.SeverityType);
        complex.PropertyWithAnnotations(e => e.MinimumValue);
        complex.PropertyWithAnnotations(e => e.MaximumValue);

        return @this;
    }
}
