using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1.Enums;

public enum ExecTypeDTO
{
    Unknown = 0,
    Ssh = 20,
    WinRm = 30,
}

public static class ForExecTypeDTO
{
    private static readonly Expression<Func<ExecType, ExecTypeDTO?>> _toDto = @in =>
        @in == ExecType.Unknown ? ExecTypeDTO.Unknown
        : @in == ExecType.Ssh ? ExecTypeDTO.Ssh
        : @in == ExecType.WinRm ? ExecTypeDTO.WinRm
        : null;

    private static readonly Expression<Func<ExecType?, ExecTypeDTO?>> _toDtoNull = @in =>
        @in == ExecType.Unknown ? ExecTypeDTO.Unknown
        : @in == ExecType.Ssh ? ExecTypeDTO.Ssh
        : @in == ExecType.WinRm ? ExecTypeDTO.WinRm
        : null;

    private static readonly Expression<Func<ExecTypeDTO?, ExecType>> _fromDto = @in =>
        @in == ExecTypeDTO.Unknown ? ExecType.Unknown
        : @in == ExecTypeDTO.Ssh ? ExecType.Ssh
        : @in == ExecTypeDTO.WinRm ? ExecType.WinRm
        : default;

    private static readonly Expression<Func<ExecTypeDTO?, ExecType?>> _fromDtoNull = @in =>
        @in == ExecTypeDTO.Unknown ? ExecType.Unknown
        : @in == ExecTypeDTO.Ssh ? ExecType.Ssh
        : @in == ExecTypeDTO.WinRm ? ExecType.WinRm
        : null;

    public static IProfileExpression AddExecType(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<ExecType, ExecTypeDTO?>();
        toDto.ConvertUsing(_toDto);

        var toDtoNull = @this.CreateMap<ExecType?, ExecTypeDTO?>();
        toDtoNull.ConvertUsing(_toDtoNull);

        var fromDto = @this.CreateMap<ExecTypeDTO?, ExecType>();
        fromDto.ConvertUsing(_fromDto);

        var fromDtoNull = @this.CreateMap<ExecTypeDTO?, ExecType?>();
        fromDtoNull.ConvertUsing(_fromDtoNull);

        return @this;
    }
}
