using EtherGizmos.SqlMonitor.Shared.OAuth.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Shared.OAuth.Services.ValueConverters;

public class ClientTypeValueConverter : ValueConverter<string?, ClientType>
{
    public ClientTypeValueConverter()
        : base(
            e => ClientTypeConverter.FromString(e),
            e => ClientTypeConverter.ToString(e))
    {
    }
}
