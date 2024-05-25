﻿using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Extensions;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1;

public class ScriptMetricDTO
{
    public DateTimeOffset? CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

    public Guid? ModifiedByUserId { get; set; }

    [Required]
    public Guid? ScriptId { get; set; }

    public ScriptDTO? Script { get; set; }

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
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        toDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        toDto.MapMember(dest => dest.ScriptId, src => src.ScriptId);
        toDto.MapMember(dest => dest.Script, src => src.Script);
        toDto.MapMember(dest => dest.MetricId, src => src.MetricId);
        toDto.MapMember(dest => dest.Metric, src => src.Metric);
        toDto.MapMember(dest => dest.ValueKey, src => src.ValueKey);
        toDto.MapMember(dest => dest.IsActive, src => src.IsActive);

        var fromDto = @this.CreateMap<ScriptMetricDTO, ScriptMetric>();
        fromDto.IgnoreAllMembers();
        /* Begin Audit */
        fromDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        fromDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        fromDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        fromDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        fromDto.MapMember(dest => dest.ScriptId, src => src.ScriptId);
        fromDto.MapMember(dest => dest.MetricId, src => src.MetricId);
        fromDto.MapMember(dest => dest.ValueKey, src => src.ValueKey);
        fromDto.MapMember(dest => dest.IsActive, src => src.IsActive);

        return @this;
    }
}
