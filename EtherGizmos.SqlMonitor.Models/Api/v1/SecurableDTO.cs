using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

[Display(Name = "Securable", GroupName = "securables")]
public class SecurableDTO
{
    [Required]
    [Display(Name = "id")]
    public string? Id { get; set; }

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
}

public static class ForSecurableDTO
{
    public static IProfileExpression AddSecurable(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<Securable, SecurableDTO>();
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

        var fromDto = @this.CreateMap<SecurableDTO, Securable>();
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

        return @this;
    }

    public static ODataModelBuilder AddSecurable(this ODataModelBuilder @this)
    {
        var entitySet = @this.EntitySetWithAnnotations<SecurableDTO>();

        var entity = @this.EntityTypeWithAnnotations<SecurableDTO>();
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

        return @this;
    }
}
