using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Models.Api.v1.Enums;

[EnumDisplay(Name = "AggregateType")]
public enum AggregateTypeDTO
{
    [Display(Name = "Unknown")]
    Unknown = -1,

    [Display(Name = "Average")]
    Average = 1,

    [Display(Name = "Maximum")]
    Maximum = 2,

    [Display(Name = "Minimum")]
    Minimum = 3,

    [Display(Name = "StandardDeviation")]
    StandardDeviation = 4,

    [Display(Name = "Sum")]
    Sum = 5,

    [Display(Name = "Variance")]
    Variance = 6,
}

public static class ForAggregateTypeDTO
{
    private static readonly Expression<Func<AggregateType, AggregateTypeDTO>> _toDto = @in =>
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

    private static readonly Expression<Func<AggregateTypeDTO, AggregateType>> _fromDto = @in =>
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
        var toDto = @this.CreateMap<AggregateType, AggregateTypeDTO>();
        toDto.ConvertUsing(_toDto);

        var toDtoNull = @this.CreateMap<AggregateType?, AggregateTypeDTO?>();
        toDtoNull.ConvertUsing(_toDtoNull);

        var fromDto = @this.CreateMap<AggregateTypeDTO, AggregateType>();
        fromDto.ConvertUsing(_fromDto);

        var fromDtoNull = @this.CreateMap<AggregateTypeDTO?, AggregateType?>();
        fromDtoNull.ConvertUsing(_fromDtoNull);

        return @this;
    }

    public static ODataModelBuilder AddAggregateType(this ODataModelBuilder @this)
    {
        var enumType = @this.EnumType/*WithAnnotations*/<AggregateTypeDTO>();
        enumType.Member/*WithAnnotations*/(AggregateTypeDTO.Unknown);
        enumType.Member/*WithAnnotations*/(AggregateTypeDTO.Average);
        enumType.Member/*WithAnnotations*/(AggregateTypeDTO.Maximum);
        enumType.Member/*WithAnnotations*/(AggregateTypeDTO.Minimum);
        enumType.Member/*WithAnnotations*/(AggregateTypeDTO.StandardDeviation);
        enumType.Member/*WithAnnotations*/(AggregateTypeDTO.Sum);
        enumType.Member/*WithAnnotations*/(AggregateTypeDTO.Variance);

        return @this;
    }
}
