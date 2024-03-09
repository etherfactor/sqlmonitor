using Asp.Versioning;
using Asp.Versioning.OData;
using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using Microsoft.OData.ModelBuilder;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Models.Api.v1.Enums;

[EnumDisplay(Name = "SeverityType")]
public enum SeverityTypeDTO
{
    Unknown = -1,
    Critical = 4,
    Error = 3,
    Nominal = 1,
    Warning = 2,
}

public class SeverityTypeDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var enumeration = builder.EnumType<SeverityTypeDTO>();

        enumeration.Namespace = "EtherGizmos.PerformancePulse";
        enumeration.Name = enumeration.Name.Replace("DTO", "");

        if (apiVersion >= ApiVersions.V0_1)
        {
        }
    }
}

public static class ForSeverityTypeDTO
{
    public static readonly Expression<Func<SeverityType, SeverityTypeDTO?>> _toDto = @in =>
        @in == SeverityType.Unknown ? SeverityTypeDTO.Unknown
        : @in == SeverityType.Critical ? SeverityTypeDTO.Critical
        : @in == SeverityType.Error ? SeverityTypeDTO.Error
        : @in == SeverityType.Nominal ? SeverityTypeDTO.Nominal
        : @in == SeverityType.Warning ? SeverityTypeDTO.Warning
        : default;

    public static readonly Expression<Func<SeverityType?, SeverityTypeDTO?>> _toDtoNull = @in =>
        @in == SeverityType.Unknown ? SeverityTypeDTO.Unknown
        : @in == SeverityType.Critical ? SeverityTypeDTO.Critical
        : @in == SeverityType.Error ? SeverityTypeDTO.Error
        : @in == SeverityType.Nominal ? SeverityTypeDTO.Nominal
        : @in == SeverityType.Warning ? SeverityTypeDTO.Warning
        : default;

    public static readonly Expression<Func<SeverityTypeDTO?, SeverityType>> _fromDto = @in =>
        @in == SeverityTypeDTO.Unknown ? SeverityType.Unknown
        : @in == SeverityTypeDTO.Critical ? SeverityType.Critical
        : @in == SeverityTypeDTO.Error ? SeverityType.Error
        : @in == SeverityTypeDTO.Nominal ? SeverityType.Nominal
        : @in == SeverityTypeDTO.Warning ? SeverityType.Warning
        : default;

    public static readonly Expression<Func<SeverityTypeDTO?, SeverityType?>> _fromDtoNull = @in =>
        @in == SeverityTypeDTO.Unknown ? SeverityType.Unknown
        : @in == SeverityTypeDTO.Critical ? SeverityType.Critical
        : @in == SeverityTypeDTO.Error ? SeverityType.Error
        : @in == SeverityTypeDTO.Nominal ? SeverityType.Nominal
        : @in == SeverityTypeDTO.Warning ? SeverityType.Warning
        : default;

    public static IProfileExpression AddSeverityType(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<SeverityType, SeverityTypeDTO?>();
        toDto.ConvertUsing(_toDto);

        var toDtoNull = @this.CreateMap<SeverityType?, SeverityTypeDTO?>();
        toDtoNull.ConvertUsing(_toDtoNull);

        var fromDto = @this.CreateMap<SeverityTypeDTO?, SeverityType>();
        fromDto.ConvertUsing(_fromDto);

        var fromDtoNull = @this.CreateMap<SeverityTypeDTO?, SeverityType?>();
        fromDtoNull.ConvertUsing(_fromDtoNull);

        return @this;
    }
}
