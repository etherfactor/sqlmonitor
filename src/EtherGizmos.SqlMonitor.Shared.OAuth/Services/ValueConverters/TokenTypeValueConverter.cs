using EtherGizmos.SqlMonitor.Shared.OAuth.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Shared.OAuth.Services.ValueConverters;

public class TokenTypeValueConverter : ValueConverter<string?, TokenType>
{
    public TokenTypeValueConverter()
        : base(
            e => TokenTypeConverter.FromString(e),
            e => TokenTypeConverter.ToString(e))
    {
    }
}
