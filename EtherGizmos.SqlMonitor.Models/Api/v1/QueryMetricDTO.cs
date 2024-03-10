﻿using Asp.Versioning;
using Asp.Versioning.OData;
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
    public Guid? MetricId { get; set; }

    [Required]
    public string? ValueExpression { get; set; }

    public List<QueryMetricSeverityDTO> Severities { get; set; } = new List<QueryMetricSeverityDTO>();

    public Task EnsureValid(IQueryable<QueryMetric> records)
    {
        return Task.CompletedTask;
    }
}

public class QueryMetricDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var complex = builder.ComplexType<QueryMetricDTO>();

        complex.Namespace = "EtherGizmos.PerformancePulse";
        complex.Name = complex.Name.Replace("DTO", "");

        complex.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            complex.Property(e => e.MetricId);
            complex.Property(e => e.ValueExpression);
            complex.CollectionProperty(e => e.Severities);
        }
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
}
