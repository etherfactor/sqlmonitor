using Asp.Versioning;
using Asp.Versioning.OData;
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
    public SeverityTypeDTO? SeverityType { get; set; }

    public string? MinimumExpression { get; set; }

    public string? MaximumExpression { get; set; }
}

public class QueryMetricSeverityDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var complex = builder.ComplexType<QueryMetricSeverityDTO>();

        complex.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            complex.EnumProperty(e => e.SeverityType);
            complex.Property(e => e.MinimumExpression);
            complex.Property(e => e.MaximumExpression);
        }
    }
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
