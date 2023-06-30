using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Api.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

[Display(Name = "Securable", GroupName = "securables")]
public class SecurableDTO : AuditableDTO
{
    [Required]
    [Display(Name = "id")]
    public string? Id { get; set; }

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
        toDto.MapAuditingColumnsToDTO();
        toDto.MapMember(e => e.Id, e => e.Id);
        toDto.MapMember(e => e.Name, e => e.Name);
        toDto.MapMember(e => e.Description, e => e.Description);

        var fromDto = @this.CreateMap<SecurableDTO, Securable>();
        fromDto.IgnoreAllMembers();
        fromDto.MapAuditingColumnsFromDTO();
        fromDto.MapMember(e => e.Id, e => e.Id);
        fromDto.MapMember(e => e.Name, e => e.Name);
        fromDto.MapMember(e => e.Description, e => e.Description);

        return @this;
    }

    public static ODataModelBuilder AddSecurable(this ODataModelBuilder @this)
    {
        var entitySet = @this.EntitySetWithAnnotations<SecurableDTO>();

        var entity = @this.EntityTypeWithAnnotations<SecurableDTO>();
        entity.HasKey(e => e.Id);
        entity.PropertyWithAnnotations(e => e.Id);
        entity.AuditPropertiesWithAnnotations();
        entity.PropertyWithAnnotations(e => e.Name);
        entity.PropertyWithAnnotations(e => e.Description);

        return @this;
    }
}
