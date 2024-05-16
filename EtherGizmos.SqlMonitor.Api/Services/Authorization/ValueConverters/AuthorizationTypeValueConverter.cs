using EtherGizmos.SqlMonitor.Models.Authorization.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Api.Services.Authorization.ValueConverters;

public class AuthorizationTypeValueConverter : ValueConverter<string?, AuthorizationType>
{
    public AuthorizationTypeValueConverter()
        : base(
            e => AuthorizationTypeConverter.FromString(e),
            e => AuthorizationTypeConverter.ToString(e))
    {
    }
}
