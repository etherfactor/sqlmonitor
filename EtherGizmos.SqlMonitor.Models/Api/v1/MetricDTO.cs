using Asp.Versioning;
using Asp.Versioning.OData;
using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Api.v1.Enums;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

[Display(Name = "Metric", GroupName = "metrics")]
public class MetricDTO
{
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

    [Required]
    [Display(Name = "name")]
    public string? Name { get; set; }

    [Display(Name = "description")]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "aggregate_type")]
    public AggregateTypeDTO? AggregateType { get; set; }

    [Display(Name = "severities")]
    public List<MetricSeverityDTO> Severities { get; set; } = new List<MetricSeverityDTO>();

    public Task EnsureValid(IQueryable<Metric> records)
    {
        return Task.CompletedTask;
    }
}

public class MetricDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var entitySet = builder.EntitySet<MetricDTO>("metrics");
        var entity = builder.EntityType<MetricDTO>();

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
            entity.Property(e => e.Name);
            entity.Property(e => e.Description);
            entity.EnumProperty(e => e.AggregateType);
            entity.CollectionProperty(e => e.Severities);
        }
    }
}

public static class ForMetricDTO
{
    public static IProfileExpression AddMetric(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<Metric, MetricDTO>();
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
        toDto.MapMember(dest => dest.AggregateType, src => src.AggregateType);
        toDto.MapMember(dest => dest.Severities, src => src.Severities);

        var fromDto = @this.CreateMap<MetricDTO, Metric>();
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
        fromDto.MapMember(dest => dest.AggregateType, src => src.AggregateType);
        fromDto.MapMember(dest => dest.Severities, src => src.Severities);

        return @this;
    }

    public static ODataModelBuilder AddMetric(this ODataModelBuilder @this)
    {
        var entitySet = @this.EntitySetWithAnnotations<MetricDTO>();

        var entity = @this.EntityTypeWithAnnotations<MetricDTO>();
        entity.HasKey(e => e.Id);
        entity.PropertyWithAnnotations(e => e.Id);
        /* Begin Audit */
        entity.PropertyWithAnnotations(e => e.CreatedAt);
        entity.PropertyWithAnnotations(e => e.CreatedByUserId);
        entity.PropertyWithAnnotations(e => e.ModifiedAt);
        entity.PropertyWithAnnotations(e => e.ModifiedByUserId);
        /*  End Audit  */
        entity.PropertyWithAnnotations(e => e.Name);
        entity.PropertyWithAnnotations(e => e.Description);
        entity.EnumPropertyWithAnnotations(e => e.AggregateType);
        entity.CollectionPropertyWithAnnotations(e => e.Severities);

        return @this;
    }
}
