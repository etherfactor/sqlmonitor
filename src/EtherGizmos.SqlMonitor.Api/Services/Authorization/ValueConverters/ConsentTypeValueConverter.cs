using EtherGizmos.SqlMonitor.Models.Authorization.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Api.Services.Authorization.ValueConverters;

public class ConsentTypeValueConverter : ValueConverter<string?, ConsentType>
{
    public ConsentTypeValueConverter()
        : base(
            e => ConsentTypeConverter.FromString(e),
            e => ConsentTypeConverter.ToString(e))
    {
    }
}
