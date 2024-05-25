﻿using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1;

public class QueryDTO
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset? CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

    public Guid? ModifiedByUserId { get; set; }

    [Required]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [Required]
    public TimeSpan? RunFrequency { get; set; }

    public DateTimeOffset? LastRunAtUtc { get; set; }

    public bool? IsActive { get; set; }

    public string? BucketColumn { get; set; }

    public string? TimestampUtcColumn { get; set; }

    public List<QueryVariantDTO> Variants { get; set; } = new();

    public List<QueryMetricDTO> Metrics { get; set; } = new();

    public Task EnsureValid(IQueryable<Query> records)
    {
        var duplicates = Metrics.Select((e, i) => new { Index = i, Value = e })
            .GroupBy(e => e.Value.MetricId)
            .Where(e => e.Count() > 1);

        if (duplicates.Any())
        {
            var values = duplicates.SelectMany(e =>
                e.Take(1).Select(v => new { First = true, Value = v }).Concat(
                    e.Skip(1).Select(v => new { First = false, Value = v })));

            var error = new ODataDuplicateReferenceError<QueryDTO>(values.Select(v =>
            {
                Expression<Func<QueryDTO, object?>> expression = e => e.Metrics[v.Value.Index].MetricId;
                return (v.First, expression);
            }).ToArray());

            throw new ReturnODataErrorException(error);
        }

        return Task.CompletedTask;
    }
}

public static class ForQueryDTO
{
    public static IProfileExpression AddQuery(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<Query, QueryDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        toDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        toDto.MapMember(dest => dest.Name, src => src.Name);
        toDto.MapMember(dest => dest.Description, src => src.Description);
        toDto.MapMember(dest => dest.RunFrequency, src => src.RunFrequency);
        toDto.MapMember(dest => dest.LastRunAtUtc, src => src.LastRunAtUtc);
        toDto.MapMember(dest => dest.IsActive, src => src.IsActive);
        toDto.MapMember(dest => dest.BucketColumn, src => src.BucketColumn);
        toDto.MapMember(dest => dest.TimestampUtcColumn, src => src.TimestampUtcColumn);
        toDto.MapMember(dest => dest.Variants, src => src.Variants);

        var fromDto = @this.CreateMap<QueryDTO, Query>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        fromDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        fromDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        fromDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        fromDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        fromDto.MapMember(dest => dest.Name, src => src.Name);
        fromDto.MapMember(dest => dest.Description, src => src.Description);
        fromDto.MapMember(dest => dest.RunFrequency, src => src.RunFrequency);
        fromDto.MapMember(dest => dest.IsActive, src => src.IsActive);
        fromDto.MapMember(dest => dest.BucketColumn, src => src.BucketColumn);
        fromDto.MapMember(dest => dest.TimestampUtcColumn, src => src.TimestampUtcColumn);
        fromDto.MapMember(dest => dest.Variants, src => src.Variants);

        return @this;
    }
}
