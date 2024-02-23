using Asp.Versioning;
using Asp.Versioning.OData;
using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

[Display(Name = "QueryInstanceDatabase")]
public class QueryInstanceDatabaseDTO
{
    [Required]
    public Guid? InstanceId { get; set; }

    public InstanceDTO? Instance { get; set; }

    [Required]
    public string? DatabaseOverride { get; set; }
}

public class QueryInstanceDatabaseDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var complex = builder.ComplexType<QueryInstanceDatabaseDTO>();

        complex.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            complex.Property(e => e.InstanceId);
            complex.HasRequired(e => e.Instance);
            complex.Property(e => e.DatabaseOverride);
        }
    }
}

public static class ForQueryInstanceDatabaseDTO
{
    public static IProfileExpression AddQueryInstanceDatabase(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<QueryInstanceDatabase, QueryInstanceDatabaseDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.InstanceId, src => src.InstanceId);
        toDto.MapMember(dest => dest.Instance, src => src.Instance);
        toDto.MapMember(dest => dest.DatabaseOverride, src => src.DatabaseOverride);

        var fromDto = @this.CreateMap<QueryInstanceDatabaseDTO, QueryInstanceDatabase>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.InstanceId, src => src.InstanceId);
        fromDto.MapMember(dest => dest.Instance, src => src.Instance);
        fromDto.MapMember(dest => dest.DatabaseOverride, src => src.DatabaseOverride);

        return @this;
    }

    public static ODataModelBuilder AddQueryInstanceDatabase(this ODataModelBuilder @this)
    {
        var complex = @this.ComplexTypeWithAnnotations<QueryInstanceDatabaseDTO>();
        complex.PropertyWithAnnotations(e => e.InstanceId);
        complex.HasRequiredWithAnnotations(e => e.Instance);
        complex.PropertyWithAnnotations(e => e.DatabaseOverride);

        return @this;
    }
}
