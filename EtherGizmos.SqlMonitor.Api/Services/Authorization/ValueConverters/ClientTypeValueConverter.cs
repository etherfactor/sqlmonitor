using EtherGizmos.SqlMonitor.Models.Authorization.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Api.Services.Authorization.ValueConverters;

public class ClientTypeValueConverter : ValueConverter<string?, ClientType>
{
    public ClientTypeValueConverter()
        : base(
            e => ClientTypeConverter.FromString(e),
            e => ClientTypeConverter.ToString(e))
    {
    }
}
