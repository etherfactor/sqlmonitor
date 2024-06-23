using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1.Enums;

public enum WinRmAuthenticationTypeDTO
{
    Unknown = 0,
    Basic = 20,
    Kerberos = 10,
}

public static class ForWinRmAuthenticationTypeDTO
{
    private static readonly Expression<Func<WinRmAuthenticationType, WinRmAuthenticationTypeDTO?>> _toDto = @in =>
        @in == WinRmAuthenticationType.Unknown ? WinRmAuthenticationTypeDTO.Unknown
        : @in == WinRmAuthenticationType.Basic ? WinRmAuthenticationTypeDTO.Basic
        : @in == WinRmAuthenticationType.Kerberos ? WinRmAuthenticationTypeDTO.Kerberos
        : null;

    private static readonly Expression<Func<WinRmAuthenticationType?, WinRmAuthenticationTypeDTO?>> _toDtoNull = @in =>
        @in == WinRmAuthenticationType.Unknown ? WinRmAuthenticationTypeDTO.Unknown
        : @in == WinRmAuthenticationType.Basic ? WinRmAuthenticationTypeDTO.Basic
        : @in == WinRmAuthenticationType.Kerberos ? WinRmAuthenticationTypeDTO.Kerberos
        : null;

    private static readonly Expression<Func<WinRmAuthenticationTypeDTO?, WinRmAuthenticationType>> _fromDto = @in =>
        @in == WinRmAuthenticationTypeDTO.Unknown ? WinRmAuthenticationType.Unknown
        : @in == WinRmAuthenticationTypeDTO.Basic ? WinRmAuthenticationType.Basic
        : @in == WinRmAuthenticationTypeDTO.Kerberos ? WinRmAuthenticationType.Kerberos
        : default;

    private static readonly Expression<Func<WinRmAuthenticationTypeDTO?, WinRmAuthenticationType?>> _fromDtoNull = @in =>
        @in == WinRmAuthenticationTypeDTO.Unknown ? WinRmAuthenticationType.Unknown
        : @in == WinRmAuthenticationTypeDTO.Basic ? WinRmAuthenticationType.Basic
        : @in == WinRmAuthenticationTypeDTO.Kerberos ? WinRmAuthenticationType.Kerberos
        : null;

    public static IProfileExpression AddWinRmAuthenticationType(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<WinRmAuthenticationType, WinRmAuthenticationTypeDTO?>();
        toDto.ConvertUsing(_toDto);

        var toDtoNull = @this.CreateMap<WinRmAuthenticationType?, WinRmAuthenticationTypeDTO?>();
        toDtoNull.ConvertUsing(_toDtoNull);

        var fromDto = @this.CreateMap<WinRmAuthenticationTypeDTO?, WinRmAuthenticationType>();
        fromDto.ConvertUsing(_fromDto);

        var fromDtoNull = @this.CreateMap<WinRmAuthenticationTypeDTO?, WinRmAuthenticationType?>();
        fromDtoNull.ConvertUsing(_fromDtoNull);

        return @this;
    }
}
