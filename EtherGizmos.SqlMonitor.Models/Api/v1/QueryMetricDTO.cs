using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

[Display(Name = "QueryMetric")]
public class QueryMetricDTO
{
    [Required]
    [Display(Name = "metric_id")]
    public Guid? MetricId { get; set; }

    [Required]
    [Display(Name = "value_expression")]
    public string? ValueExpression { get; set; }

    [Display(Name = "severities")]
    public List<QueryMetricSeverityDTO> Severities { get; set; } = new List<QueryMetricSeverityDTO>();

    public Task EnsureValid(IQueryable<QueryMetric> records)
    {
        return Task.CompletedTask;
    }
}

public static class ForQueryMetricDTO
{
    public static IProfileExpression AddQueryMetric(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<QueryMetric, QueryMetricDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.MetricId, src => src.MetricId);
        toDto.MapMember(dest => dest.ValueExpression, src => src.ValueExpression);
        toDto.MapMember(dest => dest.Severities, src => src.Severities);

        var fromDto = @this.CreateMap<QueryMetricDTO, QueryMetric>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.MetricId, src => src.MetricId);
        fromDto.MapMember(dest => dest.ValueExpression, src => src.ValueExpression);
        fromDto.MapMember(dest => dest.Severities, src => src.Severities);

        return @this;
    }

    public static ODataModelBuilder AddQueryMetric(this ODataModelBuilder @this)
    {
        var complex = @this.ComplexType<QueryMetricDTO>();
        complex.PropertyWithAnnotations(e => e.MetricId);
        complex.PropertyWithAnnotations(e => e.ValueExpression);
        complex.CollectionPropertyWithAnnotations(e => e.Severities);

        return @this;
    }
}
