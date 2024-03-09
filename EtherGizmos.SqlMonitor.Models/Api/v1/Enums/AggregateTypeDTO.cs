using Asp.Versioning;
using Asp.Versioning.OData;
using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using Microsoft.OData.ModelBuilder;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Models.Api.v1.Enums;

public enum AggregateTypeDTO
{
    Unknown = -1,
    Average = 1,
    Maximum = 2,
    Minimum = 3,
    StandardDeviation = 4,
    Sum = 5,
    Variance = 6,
}

public class AggregateTypeDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var enumeration = builder.EnumType<AggregateTypeDTO>();

        enumeration.Namespace = "EtherGizmos.PerformancePulse";
        enumeration.Name = enumeration.Name.Replace("DTO", "");

        if (apiVersion >= ApiVersions.V0_1)
        {
        }
    }
}

public static class ForAggregateTypeDTO
{
    private static readonly Expression<Func<AggregateType, AggregateTypeDTO?>> _toDto = @in =>
        @in == AggregateType.Unknown ? AggregateTypeDTO.Unknown
        : @in == AggregateType.Average ? AggregateTypeDTO.Average
        : @in == AggregateType.Maximum ? AggregateTypeDTO.Maximum
        : @in == AggregateType.Minimum ? AggregateTypeDTO.Minimum
        : @in == AggregateType.StandardDeviation ? AggregateTypeDTO.StandardDeviation
        : @in == AggregateType.Sum ? AggregateTypeDTO.Sum
        : @in == AggregateType.Variance ? AggregateTypeDTO.Variance
        : default;

    private static readonly Expression<Func<AggregateType?, AggregateTypeDTO?>> _toDtoNull = @in =>
        @in == AggregateType.Unknown ? AggregateTypeDTO.Unknown
        : @in == AggregateType.Average ? AggregateTypeDTO.Average
        : @in == AggregateType.Maximum ? AggregateTypeDTO.Maximum
        : @in == AggregateType.Minimum ? AggregateTypeDTO.Minimum
        : @in == AggregateType.StandardDeviation ? AggregateTypeDTO.StandardDeviation
        : @in == AggregateType.Sum ? AggregateTypeDTO.Sum
        : @in == AggregateType.Variance ? AggregateTypeDTO.Variance
        : default;

    private static readonly Expression<Func<AggregateTypeDTO?, AggregateType>> _fromDto = @in =>
        @in == AggregateTypeDTO.Unknown ? AggregateType.Unknown
        : @in == AggregateTypeDTO.Average ? AggregateType.Average
        : @in == AggregateTypeDTO.Maximum ? AggregateType.Maximum
        : @in == AggregateTypeDTO.Minimum ? AggregateType.Minimum
        : @in == AggregateTypeDTO.StandardDeviation ? AggregateType.StandardDeviation
        : @in == AggregateTypeDTO.Sum ? AggregateType.Sum
        : @in == AggregateTypeDTO.Variance ? AggregateType.Variance
        : default;

    private static readonly Expression<Func<AggregateTypeDTO?, AggregateType?>> _fromDtoNull = @in =>
        @in == AggregateTypeDTO.Unknown ? AggregateType.Unknown
        : @in == AggregateTypeDTO.Average ? AggregateType.Average
        : @in == AggregateTypeDTO.Maximum ? AggregateType.Maximum
        : @in == AggregateTypeDTO.Minimum ? AggregateType.Minimum
        : @in == AggregateTypeDTO.StandardDeviation ? AggregateType.StandardDeviation
        : @in == AggregateTypeDTO.Sum ? AggregateType.Sum
        : @in == AggregateTypeDTO.Variance ? AggregateType.Variance
        : default;

    public static IProfileExpression AddAggregateType(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<AggregateType, AggregateTypeDTO?>();
        toDto.ConvertUsing(_toDto);

        var toDtoNull = @this.CreateMap<AggregateType?, AggregateTypeDTO?>();
        toDtoNull.ConvertUsing(_toDtoNull);

        var fromDto = @this.CreateMap<AggregateTypeDTO?, AggregateType>();
        fromDto.ConvertUsing(_fromDto);

        var fromDtoNull = @this.CreateMap<AggregateTypeDTO?, AggregateType?>();
        fromDtoNull.ConvertUsing(_fromDtoNull);

        return @this;
    }
}
