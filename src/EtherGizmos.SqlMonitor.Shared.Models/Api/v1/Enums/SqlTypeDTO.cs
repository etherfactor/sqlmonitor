using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1.Enums;

public enum SqlTypeDTO
{
    Unknown = 0,
    MicrosoftSqlServer = 10,
    MySql = 20,
    MariaDb = 25,
    PostgreSql = 30,
}

public static class ForSqlTypeDTO
{
    private static readonly Expression<Func<SqlType, SqlTypeDTO?>> _toDto = @in =>
        @in == SqlType.Unknown ? SqlTypeDTO.Unknown
        : @in == SqlType.SqlServer ? SqlTypeDTO.MicrosoftSqlServer
        : @in == SqlType.MySql ? SqlTypeDTO.MySql
        : @in == SqlType.MariaDb ? SqlTypeDTO.MariaDb
        : @in == SqlType.PostgreSql ? SqlTypeDTO.PostgreSql
        : default;

    private static readonly Expression<Func<SqlType?, SqlTypeDTO?>> _toDtoNull = @in =>
        @in == SqlType.Unknown ? SqlTypeDTO.Unknown
        : @in == SqlType.SqlServer ? SqlTypeDTO.MicrosoftSqlServer
        : @in == SqlType.MySql ? SqlTypeDTO.MySql
        : @in == SqlType.MariaDb ? SqlTypeDTO.MariaDb
        : @in == SqlType.PostgreSql ? SqlTypeDTO.PostgreSql
        : default;

    private static readonly Expression<Func<SqlTypeDTO?, SqlType>> _fromDto = @in =>
        @in == SqlTypeDTO.Unknown ? SqlType.Unknown
        : @in == SqlTypeDTO.MicrosoftSqlServer ? SqlType.SqlServer
        : @in == SqlTypeDTO.MySql ? SqlType.MySql
        : @in == SqlTypeDTO.MariaDb ? SqlType.MariaDb
        : @in == SqlTypeDTO.PostgreSql ? SqlType.PostgreSql
        : default;

    private static readonly Expression<Func<SqlTypeDTO?, SqlType?>> _fromDtoNull = @in =>
        @in == SqlTypeDTO.Unknown ? SqlType.Unknown
        : @in == SqlTypeDTO.MicrosoftSqlServer ? SqlType.SqlServer
        : @in == SqlTypeDTO.MySql ? SqlType.MySql
        : @in == SqlTypeDTO.MariaDb ? SqlType.MariaDb
        : @in == SqlTypeDTO.PostgreSql ? SqlType.PostgreSql
        : default;

    public static IProfileExpression AddSqlType(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<SqlType, SqlTypeDTO?>();
        toDto.ConvertUsing(_toDto);

        var toDtoNull = @this.CreateMap<SqlType?, SqlTypeDTO?>();
        toDtoNull.ConvertUsing(_toDtoNull);

        var fromDto = @this.CreateMap<SqlTypeDTO?, SqlType>();
        fromDto.ConvertUsing(_fromDto);

        var fromDtoNull = @this.CreateMap<SqlTypeDTO?, SqlType?>();
        fromDtoNull.ConvertUsing(_fromDtoNull);

        return @this;
    }
}
