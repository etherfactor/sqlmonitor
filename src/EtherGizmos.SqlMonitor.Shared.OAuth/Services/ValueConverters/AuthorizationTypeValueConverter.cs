using EtherGizmos.SqlMonitor.Shared.OAuth.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Shared.OAuth.Services.ValueConverters;

public class AuthorizationTypeValueConverter : ValueConverter<string?, AuthorizationType>
{
    public AuthorizationTypeValueConverter()
        : base(
            e => AuthorizationTypeConverter.FromString(e),
            e => AuthorizationTypeConverter.ToString(e))
    {
    }
}
