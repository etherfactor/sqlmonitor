using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Extensions;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1;

public class ScriptMetricDTO
{
    [Required]
    public int? MetricId { get; set; }

    public MetricDTO? Metric { get; set; }

    [Required]
    public string? ValueKey { get; set; }

    public bool? IsActive { get; set; }
}

public static class ForScriptMetricDTO
{
    public static IProfileExpression AddScriptMetric(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<ScriptMetric, ScriptMetricDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.MetricId, src => src.MetricId);
        toDto.MapMember(dest => dest.Metric, src => src.Metric);
        toDto.MapMember(dest => dest.ValueKey, src => src.ValueKey);
        toDto.MapMember(dest => dest.IsActive, src => src.IsActive);

        var fromDto = @this.CreateMap<ScriptMetricDTO, ScriptMetric>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.MetricId, src => src.MetricId);
        fromDto.MapMember(dest => dest.ValueKey, src => src.ValueKey);
        fromDto.MapMember(dest => dest.IsActive, src => src.IsActive);

        return @this;
    }
}
