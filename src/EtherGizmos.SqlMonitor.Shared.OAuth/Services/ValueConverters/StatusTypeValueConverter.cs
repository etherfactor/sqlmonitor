using EtherGizmos.SqlMonitor.Shared.OAuth.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Shared.OAuth.Services.ValueConverters;

public class StatusTypeValueConverter : ValueConverter<string?, StatusType>
{
    public StatusTypeValueConverter()
        : base(
            e => StatusTypeConverter.FromString(e),
            e => StatusTypeConverter.ToString(e))
    {
    }
}
