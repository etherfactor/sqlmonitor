using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1.Enums;

public enum SshAuthenticationTypeDTO
{
    Unknown = 0,
    None = 1,
    Password = 10,
    PrivateKey = 20,
}

public static class ForSshAuthenticationTypeDTO
{
    private static readonly Expression<Func<SshAuthenticationType, SshAuthenticationTypeDTO?>> _toDto = @in =>
        @in == SshAuthenticationType.Unknown ? SshAuthenticationTypeDTO.Unknown
        : @in == SshAuthenticationType.None ? SshAuthenticationTypeDTO.None
        : @in == SshAuthenticationType.Password ? SshAuthenticationTypeDTO.Password
        : @in == SshAuthenticationType.PrivateKey ? SshAuthenticationTypeDTO.PrivateKey
        : null;

    private static readonly Expression<Func<SshAuthenticationType?, SshAuthenticationTypeDTO?>> _toDtoNull = @in =>
        @in == SshAuthenticationType.Unknown ? SshAuthenticationTypeDTO.Unknown
        : @in == SshAuthenticationType.None ? SshAuthenticationTypeDTO.None
        : @in == SshAuthenticationType.Password ? SshAuthenticationTypeDTO.Password
        : @in == SshAuthenticationType.PrivateKey ? SshAuthenticationTypeDTO.PrivateKey
        : null;

    private static readonly Expression<Func<SshAuthenticationTypeDTO?, SshAuthenticationType>> _fromDto = @in =>
        @in == SshAuthenticationTypeDTO.Unknown ? SshAuthenticationType.Unknown
        : @in == SshAuthenticationTypeDTO.None ? SshAuthenticationType.None
        : @in == SshAuthenticationTypeDTO.Password ? SshAuthenticationType.Password
        : @in == SshAuthenticationTypeDTO.PrivateKey ? SshAuthenticationType.PrivateKey
        : default;

    private static readonly Expression<Func<SshAuthenticationTypeDTO?, SshAuthenticationType?>> _fromDtoNull = @in =>
        @in == SshAuthenticationTypeDTO.Unknown ? SshAuthenticationType.Unknown
        : @in == SshAuthenticationTypeDTO.None ? SshAuthenticationType.None
        : @in == SshAuthenticationTypeDTO.Password ? SshAuthenticationType.Password
        : @in == SshAuthenticationTypeDTO.PrivateKey ? SshAuthenticationType.PrivateKey
        : null;

    public static IProfileExpression AddSshAuthenticationType(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<SshAuthenticationType, SshAuthenticationTypeDTO?>();
        toDto.ConvertUsing(_toDto);

        var toDtoNull = @this.CreateMap<SshAuthenticationType?, SshAuthenticationTypeDTO?>();
        toDtoNull.ConvertUsing(_toDtoNull);

        var fromDto = @this.CreateMap<SshAuthenticationTypeDTO?, SshAuthenticationType>();
        fromDto.ConvertUsing(_fromDto);

        var fromDtoNull = @this.CreateMap<SshAuthenticationTypeDTO?, SshAuthenticationType?>();
        fromDtoNull.ConvertUsing(_fromDtoNull);

        return @this;
    }
}
