using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

[Display(Name = "Instance", GroupName = "instances")]
public class InstanceDTO
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

    [Display(Name = "is_active")]
    public bool? IsActive { get; set; } = true;

    [Required]
    [Display(Name = "address")]
    public string? Address { get; set; }

    [Display(Name = "port")]
    public short? Port { get; set; }

    [Display(Name = "database")]
    public string? Database { get; set; }

    public Task EnsureValid(IQueryable<Instance> records)
    {
        return Task.CompletedTask;
    }
}

public static class ForInstanceDTO
{
    public static IProfileExpression AddInstance(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<Instance, InstanceDTO>();
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
        toDto.MapMember(dest => dest.IsActive, src => src.IsActive);
        toDto.MapMember(dest => dest.Address, src => src.Address);
        toDto.MapMember(dest => dest.Port, src => src.Port);
        toDto.MapMember(dest => dest.Database, src => src.Database);

        var fromDto = @this.CreateMap<InstanceDTO, Instance>();
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
        fromDto.MapMember(dest => dest.IsActive, src => src.IsActive);
        fromDto.MapMember(dest => dest.Address, src => src.Address);
        fromDto.MapMember(dest => dest.Port, src => src.Port);
        fromDto.MapMember(dest => dest.Database, src => src.Database);

        return @this;
    }

    public static ODataModelBuilder AddInstance(this ODataModelBuilder @this)
    {
        var entitySet = @this.EntitySetWithAnnotations<InstanceDTO>();

        var entity = @this.EntityTypeWithAnnotations<InstanceDTO>();
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
        entity.PropertyWithAnnotations(e => e.IsActive);
        entity.PropertyWithAnnotations(e => e.Address);
        entity.PropertyWithAnnotations(e => e.Port);
        entity.PropertyWithAnnotations(e => e.Database);

        return @this;
    }
}
