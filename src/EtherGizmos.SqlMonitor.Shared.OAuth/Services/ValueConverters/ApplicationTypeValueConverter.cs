using EtherGizmos.SqlMonitor.Shared.OAuth.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Shared.OAuth.Services.ValueConverters;

public class ApplicationTypeValueConverter : ValueConverter<string?, ApplicationType>
{
    public ApplicationTypeValueConverter()
        : base(
            e => ApplicationTypeConverter.FromString(e),
            e => ApplicationTypeConverter.ToString(e))
    {
    }
}
