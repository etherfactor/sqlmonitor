using Asp.Versioning.OData;
using Asp.Versioning;
using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Database;
using Microsoft.OData.ModelBuilder;
using EtherGizmos.SqlMonitor.Models.Extensions;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

public class QueryMetricDTO
{
    public DateTimeOffset? CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

    public Guid? ModifiedByUserId { get; set; }

    [Required]
    public Guid? QueryId { get; set; }

    public QueryDTO? Query { get; set; }

    [Required]
    public int? MetricId { get; set; }

    public MetricDTO? Metric { get; set; }

    [Required]
    public string? ValueColumn { get; set; }

    public bool? IsActive { get; set; }
}

public class QueryMetricDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var entity = builder.ComplexType<QueryMetricDTO>();

        entity.Namespace = "EtherGizmos.PerformancePulse";
        entity.Name = entity.Name.Replace("DTO", "");

        entity.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            /* Begin Audit */
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.CreatedByUserId);
            entity.Property(e => e.ModifiedAt);
            entity.Property(e => e.ModifiedByUserId);
            /*  End Audit  */
            entity.Property(e => e.QueryId);
            entity.HasRequired(e => e.Query);
            entity.Property(e => e.MetricId);
            entity.HasRequired(e => e.Metric);
            entity.Property(e => e.ValueColumn);
            entity.Property(e => e.IsActive);
        }
    }
}

public static class ForQueryMetricDTO
{
    public static IProfileExpression AddQueryMetric(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<QueryMetric, QueryMetricDTO>();
        toDto.IgnoreAllMembers();
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        toDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        toDto.MapMember(dest => dest.QueryId, src => src.QueryId);
        toDto.MapMember(dest => dest.Query, src => src.Query);
        toDto.MapMember(dest => dest.MetricId, src => src.MetricId);
        toDto.MapMember(dest => dest.Metric, src => src.Metric);
        toDto.MapMember(dest => dest.ValueColumn, src => src.ValueColumn);
        toDto.MapMember(dest => dest.IsActive, src => src.IsActive);

        var fromDto = @this.CreateMap<QueryMetricDTO, QueryMetric>();
        fromDto.IgnoreAllMembers();
        /* Begin Audit */
        fromDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        fromDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        fromDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        fromDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        fromDto.MapMember(dest => dest.QueryId, src => src.QueryId);
        fromDto.MapMember(dest => dest.MetricId, src => src.MetricId);
        fromDto.MapMember(dest => dest.ValueColumn, src => src.ValueColumn);
        fromDto.MapMember(dest => dest.IsActive, src => src.IsActive);

        return @this;
    }
}
