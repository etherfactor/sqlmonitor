using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Api.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

public class SecurableDTO : AuditableDTO
{
    [Required]
    public string? Id { get; set; }

    [Required]
    public string? Name { get; set; }

    public string? Description { get; set; }
}

public static class ForSecurableDTO
{
    public static IProfileExpression AddSecurableDTO(this IProfileExpression @this)
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

    public static ODataModelBuilder Test(this ODataModelBuilder @this)
    {
        ODataModelBuilder builder = new ODataModelBuilder();
        
        var entity = builder.EntityTypeWithAnnotations<SecurableDTO>();
        entity.HasKey(e => e.Id);
        entity.PropertyWithAnnotations(e => e.Id);
        entity.AuditPropertiesWithAnnotations();
        entity.PropertyWithAnnotations(e => e.Name);
        entity.PropertyWithAnnotations(e => e.Description);

        return @this;
    }
}
