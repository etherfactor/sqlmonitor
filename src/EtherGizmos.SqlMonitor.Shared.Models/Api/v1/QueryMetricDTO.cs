using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Extensions;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1;

public class QueryMetricDTO
{
    public DateTimeOffset? CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

    public Guid? ModifiedByUserId { get; set; }

    [Required]
    public int? MetricId { get; set; }

    public MetricDTO? Metric { get; set; }

    [Required]
    public string? ValueColumn { get; set; }

    public bool? IsActive { get; set; }
}

public static class ForQueryMetricDTO
{
    public static IProfileExpression AddQueryMetric(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<QueryMetric, QueryMetricDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.MetricId, src => src.MetricId);
        toDto.MapMember(dest => dest.Metric, src => src.Metric);
        toDto.MapMember(dest => dest.ValueColumn, src => src.ValueColumn);
        toDto.MapMember(dest => dest.IsActive, src => src.IsActive);

        var fromDto = @this.CreateMap<QueryMetricDTO, QueryMetric>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.MetricId, src => src.MetricId);
        fromDto.MapMember(dest => dest.ValueColumn, src => src.ValueColumn);
        fromDto.MapMember(dest => dest.IsActive, src => src.IsActive);

        return @this;
    }
}
