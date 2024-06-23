using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1.Enums;

public enum AggregateTypeDTO
{
    Unknown = 0,
    Sum = 10,
    Average = 20,
    Minimum = 30,
    Maximum = 35,
}

public static class ForAggregateTypeDTO
{
    private static readonly Expression<Func<AggregateType, AggregateTypeDTO?>> _toDto = @in =>
        @in == AggregateType.Unknown ? AggregateTypeDTO.Unknown
        : @in == AggregateType.Average ? AggregateTypeDTO.Average
        : @in == AggregateType.Maximum ? AggregateTypeDTO.Maximum
        : @in == AggregateType.Minimum ? AggregateTypeDTO.Minimum
        : @in == AggregateType.Sum ? AggregateTypeDTO.Sum
        : null;

    private static readonly Expression<Func<AggregateType?, AggregateTypeDTO?>> _toDtoNull = @in =>
        @in == AggregateType.Unknown ? AggregateTypeDTO.Unknown
        : @in == AggregateType.Average ? AggregateTypeDTO.Average
        : @in == AggregateType.Maximum ? AggregateTypeDTO.Maximum
        : @in == AggregateType.Minimum ? AggregateTypeDTO.Minimum
        : @in == AggregateType.Sum ? AggregateTypeDTO.Sum
        : null;

    private static readonly Expression<Func<AggregateTypeDTO?, AggregateType>> _fromDto = @in =>
        @in == AggregateTypeDTO.Unknown ? AggregateType.Unknown
        : @in == AggregateTypeDTO.Average ? AggregateType.Average
        : @in == AggregateTypeDTO.Maximum ? AggregateType.Maximum
        : @in == AggregateTypeDTO.Minimum ? AggregateType.Minimum
        : @in == AggregateTypeDTO.Sum ? AggregateType.Sum
        : default;

    private static readonly Expression<Func<AggregateTypeDTO?, AggregateType?>> _fromDtoNull = @in =>
        @in == AggregateTypeDTO.Unknown ? AggregateType.Unknown
        : @in == AggregateTypeDTO.Average ? AggregateType.Average
        : @in == AggregateTypeDTO.Maximum ? AggregateType.Maximum
        : @in == AggregateTypeDTO.Minimum ? AggregateType.Minimum
        : @in == AggregateTypeDTO.Sum ? AggregateType.Sum
        : null;

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
