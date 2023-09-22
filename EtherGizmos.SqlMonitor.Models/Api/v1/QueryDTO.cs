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
    [Display(Name = "id")]
    public Guid Id { get; set; }

    [Display(Name = "created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    [Display(Name = "created_by_user_id")]
    public Guid? CreatedByUserId { get; set; }

    [Display(Name = "modified_at")]
    public DateTimeOffset? ModifiedAt { get; set; }

    [Display(Name = "modified_by_user_id")]
    public Guid? ModifiedByUserId { get; set; }

    [Display(Name = "system_id")]
    public Guid SystemId { get; set; }

    [Required]
    [Display(Name = "name")]
    public string? Name { get; set; }

    [Display(Name = "description")]
    public string? Description { get; set; }

    [Display(Name = "is_active")]
    public bool? IsActive { get; set; } = true;

    [Required]
    [Display(Name = "sql_text")]
    public string? SqlText { get; set; }

    [Required]
    [Display(Name = "run_frequency")]
    public TimeSpan? RunFrequency { get; set; }

    [Display(Name = "last_run_at_utc")]
    public DateTimeOffset? LastRunAtUtc { get; set; }

    [Display(Name = "timestamp_utc_expression")]
    public string? TimestampUtcExpression { get; set; }

    [Display(Name = "bucket_expression")]
    public string? BucketExpression { get; set; }

    [Display(Name = "instance_blacklists")]
    public List<QueryInstanceDTO> InstanceBlacklists { get; set; } = new List<QueryInstanceDTO>();

    [Display(Name = "instance_whitelists")]
    public List<QueryInstanceDTO> InstanceWhitelists { get; set; } = new List<QueryInstanceDTO>();

    [Display(Name = "instance_database_overrides")]
    public List<QueryInstanceDatabaseDTO> InstanceDatabaseOverrides { get; set; } = new List<QueryInstanceDatabaseDTO>();

    public Task EnsureValid(IQueryable<Query> records)
    {
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
        toDto.MapMember(dest => dest.SystemId, src => src.SystemId);
        toDto.MapMember(dest => dest.Name, src => src.Name);
        toDto.MapMember(dest => dest.Description, src => src.Description);
        toDto.MapMember(dest => dest.IsActive, src => src.IsActive);
        toDto.MapMember(dest => dest.SqlText, src => src.SqlText);
        toDto.MapMember(dest => dest.RunFrequency, src => src.RunFrequency);
        toDto.MapMember(dest => dest.LastRunAtUtc, src => src.LastRunAtUtc);
        toDto.MapMember(dest => dest.TimestampUtcExpression, src => src.TimestampUtcExpression);
        toDto.MapMember(dest => dest.BucketExpression, src => src.BucketExpression);
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
        fromDto.MapMember(dest => dest.LastRunAtUtc, src => src.LastRunAtUtc);
        fromDto.MapMember(dest => dest.TimestampUtcExpression, src => src.TimestampUtcExpression);
        fromDto.MapMember(dest => dest.BucketExpression, src => src.BucketExpression);
        fromDto.MapMember(dest => dest.InstanceBlacklists, src => src.InstanceBlacklists);
        fromDto.MapMember(dest => dest.InstanceWhitelists, src => src.InstanceWhitelists);
        fromDto.MapMember(dest => dest.InstanceDatabaseOverrides, src => src.InstanceDatabaseOverrides);

        return @this;
    }

    public static ODataModelBuilder AddQuery(this ODataModelBuilder @this)
    {
        var entitySet = @this.EntitySetWithAnnotations<QueryDTO>();

        var entity = @this.EntityTypeWithAnnotations<QueryDTO>();
        entity.HasKey(e => e.Id);
        entity.PropertyWithAnnotations(e => e.Id);
        /* Begin Audit */
        entity.PropertyWithAnnotations(e => e.CreatedAt);
        entity.PropertyWithAnnotations(e => e.CreatedByUserId);
        entity.PropertyWithAnnotations(e => e.ModifiedAt);
        entity.PropertyWithAnnotations(e => e.ModifiedByUserId);
        /*  End Audit  */
        entity.PropertyWithAnnotations(e => e.SystemId);
        entity.PropertyWithAnnotations(e => e.Name);
        entity.PropertyWithAnnotations(e => e.Description);
        entity.PropertyWithAnnotations(e => e.IsActive);
        entity.PropertyWithAnnotations(e => e.SqlText);
        entity.PropertyWithAnnotations(e => e.RunFrequency);
        entity.PropertyWithAnnotations(e => e.LastRunAtUtc);
        entity.PropertyWithAnnotations(e => e.TimestampUtcExpression);
        entity.PropertyWithAnnotations(e => e.BucketExpression);
        entity.CollectionPropertyWithAnnotations(e => e.InstanceBlacklists);
        entity.CollectionPropertyWithAnnotations(e => e.InstanceWhitelists);
        entity.CollectionPropertyWithAnnotations(e => e.InstanceDatabaseOverrides);

        return @this;
    }
}
