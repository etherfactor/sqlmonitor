using Asp.Versioning;
using Asp.Versioning.OData;
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
    public SeverityTypeDTO? SeverityType { get; set; }

    public double? MinimumValue { get; set; }

    public double? MaximumValue { get; set; }
}

public class MetricSeverityDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var complex = builder.ComplexType<MetricSeverityDTO>();

        complex.Namespace = "EtherGizmos.PerformancePulse";
        complex.Name = complex.Name.Replace("DTO", "");

        complex.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            complex.EnumProperty(e => e.SeverityType);
            complex.Property(e => e.MinimumValue);
            complex.Property(e => e.MaximumValue);
        }
    }
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
}
