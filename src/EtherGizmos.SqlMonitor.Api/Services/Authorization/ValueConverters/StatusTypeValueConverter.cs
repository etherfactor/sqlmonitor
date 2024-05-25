using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Api.Services.Authorization.ValueConverters;

public class StatusTypeValueConverter : ValueConverter<string?, StatusType>
{
    public StatusTypeValueConverter()
        : base(
            e => StatusTypeConverter.FromString(e),
            e => StatusTypeConverter.ToString(e))
    {
    }
}
