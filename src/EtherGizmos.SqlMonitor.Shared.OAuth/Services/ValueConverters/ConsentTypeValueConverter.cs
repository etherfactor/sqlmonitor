using EtherGizmos.SqlMonitor.Shared.OAuth.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Shared.OAuth.Services.ValueConverters;

public class ConsentTypeValueConverter : ValueConverter<string?, ConsentType>
{
    public ConsentTypeValueConverter()
        : base(
            e => ConsentTypeConverter.FromString(e),
            e => ConsentTypeConverter.ToString(e))
    {
    }
}
