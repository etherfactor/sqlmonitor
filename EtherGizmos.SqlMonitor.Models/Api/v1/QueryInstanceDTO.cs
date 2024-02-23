using Asp.Versioning;
using Asp.Versioning.OData;
using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

[Display(Name = "QueryInstance")]
public class QueryInstanceDTO
{
    [Required]
    public Guid? InstanceId { get; set; }

    public InstanceDTO? Instance { get; set; }
}

public class QueryInstanceDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var complex = builder.ComplexType<QueryInstanceDTO>();

        complex.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            complex.Property(e => e.InstanceId);
            complex.HasRequired(e => e.Instance);
        }
    }
}

public static class ForQueryInstanceDTO
{
    public static IProfileExpression AddQueryInstance(this IProfileExpression @this)
    {
        var toDtoB = @this.CreateMap<QueryInstanceBlacklist, QueryInstanceDTO>();
        toDtoB.IgnoreAllMembers();
        toDtoB.MapMember(dest => dest.InstanceId, src => src.InstanceId);
        toDtoB.MapMember(dest => dest.Instance, src => src.Instance);

        var fromDtoB = @this.CreateMap<QueryInstanceDTO, QueryInstanceBlacklist>();
        fromDtoB.IgnoreAllMembers();
        fromDtoB.MapMember(dest => dest.InstanceId, src => src.InstanceId);
        fromDtoB.MapMember(dest => dest.Instance, src => src.Instance);

        var toDtoW = @this.CreateMap<QueryInstanceWhitelist, QueryInstanceDTO>();
        toDtoW.IgnoreAllMembers();
        toDtoW.MapMember(dest => dest.InstanceId, src => src.InstanceId);
        toDtoW.MapMember(dest => dest.Instance, src => src.Instance);

        var fromDtoW = @this.CreateMap<QueryInstanceDTO, QueryInstanceWhitelist>();
        fromDtoW.IgnoreAllMembers();
        fromDtoW.MapMember(dest => dest.InstanceId, src => src.InstanceId);
        fromDtoW.MapMember(dest => dest.Instance, src => src.Instance);

        return @this;
    }

    public static ODataModelBuilder AddQueryInstance(this ODataModelBuilder @this)
    {
        var complex = @this.ComplexTypeWithAnnotations<QueryInstanceDTO>();
        complex.PropertyWithAnnotations(e => e.InstanceId);
        complex.HasRequiredWithAnnotations(e => e.Instance);

        return @this;
    }
}
