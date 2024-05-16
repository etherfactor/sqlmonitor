using EtherGizmos.SqlMonitor.Models.Authorization.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Api.Services.Authorization.ValueConverters;

public class TokenTypeValueConverter : ValueConverter<string?, TokenType>
{
    public TokenTypeValueConverter()
        : base(
            e => TokenTypeConverter.FromString(e),
            e => TokenTypeConverter.ToString(e))
    {
    }
}
