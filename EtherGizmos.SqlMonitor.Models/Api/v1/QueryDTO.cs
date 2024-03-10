using Asp.Versioning;
using Asp.Versioning.OData;
using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

[Display(Name = "Query", GroupName = "queries")]
public class QueryDTO
{
    [Required]
    public Guid Id { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

    public Guid? ModifiedByUserId { get; set; }

    public Guid SystemId { get; set; }

    [Required]
    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; } = true;

    [Required]
    public string? SqlText { get; set; }

    [Required]
    public TimeSpan? RunFrequency { get; set; }

    public DateTimeOffset? LastRunAt { get; set; }

    public string? TimestampUtcExpression { get; set; }

    public string? BucketExpression { get; set; }

    public List<QueryMetricDTO> Metrics { get; set; } = new List<QueryMetricDTO>();

    public List<QueryInstanceDTO> InstanceBlacklists { get; set; } = new List<QueryInstanceDTO>();

    public List<QueryInstanceDTO> InstanceWhitelists { get; set; } = new List<QueryInstanceDTO>();

    public List<QueryInstanceDatabaseDTO> InstanceDatabaseOverrides { get; set; } = new List<QueryInstanceDatabaseDTO>();

    public Task EnsureValid(IQueryable<Query> records)
    {
        return Task.CompletedTask;
    }
}

public class QueryDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var entitySet = builder.EntitySet<QueryDTO>("queries");
        var entity = builder.EntityType<QueryDTO>();

        entity.Namespace = "EtherGizmos.PerformancePulse";
        entity.Name = entity.Name.Replace("DTO", "");

        entity.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id);
            /* Begin Audit */
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.CreatedByUserId);
            entity.Property(e => e.ModifiedAt);
            entity.Property(e => e.ModifiedByUserId);
            /*  End Audit  */
            entity.Property(e => e.SystemId);
            entity.Property(e => e.Name);
            entity.Property(e => e.Description);
            entity.Property(e => e.IsActive);
            entity.Property(e => e.SqlText);
            entity.Property(e => e.RunFrequency);
            entity.Property(e => e.LastRunAt);
            entity.Property(e => e.TimestampUtcExpression);
            entity.Property(e => e.BucketExpression);
            entity.CollectionProperty(e => e.Metrics);
            entity.CollectionProperty(e => e.InstanceBlacklists);
            entity.CollectionProperty(e => e.InstanceWhitelists);
            entity.CollectionProperty(e => e.InstanceDatabaseOverrides);
        }
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
        toDto.MapMember(dest => dest.SystemId, src => src.SystemId);
        toDto.MapMember(dest => dest.Name, src => src.Name);
        toDto.MapMember(dest => dest.Description, src => src.Description);
        toDto.MapMember(dest => dest.IsActive, src => src.IsActive);
        toDto.MapMember(dest => dest.SqlText, src => src.SqlText);
        toDto.MapMember(dest => dest.RunFrequency, src => src.RunFrequency);
        toDto.MapMember(dest => dest.LastRunAt, src => src.LastRunAtUtc);
        toDto.MapMember(dest => dest.TimestampUtcExpression, src => src.TimestampUtcExpression);
        toDto.MapMember(dest => dest.BucketExpression, src => src.BucketExpression);
        toDto.MapMember(dest => dest.Metrics, src => src.Metrics);
        toDto.MapMember(dest => dest.InstanceBlacklists, src => src.InstanceBlacklists);
        toDto.MapMember(dest => dest.InstanceWhitelists, src => src.InstanceWhitelists);
        toDto.MapMember(dest => dest.InstanceDatabaseOverrides, src => src.InstanceDatabaseOverrides);

        var fromDto = @this.CreateMap<QueryDTO, Query>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        fromDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        fromDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        fromDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        fromDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        fromDto.MapMember(dest => dest.SystemId, src => src.SystemId);
        fromDto.MapMember(dest => dest.Name, src => src.Name);
        fromDto.MapMember(dest => dest.Description, src => src.Description);
        fromDto.MapMember(dest => dest.IsActive, src => src.IsActive);
        fromDto.MapMember(dest => dest.SqlText, src => src.SqlText);
        fromDto.MapMember(dest => dest.RunFrequency, src => src.RunFrequency);
        fromDto.MapMember(dest => dest.LastRunAtUtc, src => src.LastRunAt);
        fromDto.MapMember(dest => dest.TimestampUtcExpression, src => src.TimestampUtcExpression);
        fromDto.MapMember(dest => dest.BucketExpression, src => src.BucketExpression);
        fromDto.MapMember(dest => dest.Metrics, src => src.Metrics);
        fromDto.MapMember(dest => dest.InstanceBlacklists, src => src.InstanceBlacklists);
        fromDto.MapMember(dest => dest.InstanceWhitelists, src => src.InstanceWhitelists);
        fromDto.MapMember(dest => dest.InstanceDatabaseOverrides, src => src.InstanceDatabaseOverrides);

        return @this;
    }
}
