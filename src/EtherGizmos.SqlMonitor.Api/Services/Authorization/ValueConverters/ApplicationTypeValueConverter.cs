using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Api.Services.Authorization.ValueConverters;

public class ApplicationTypeValueConverter : ValueConverter<string?, ApplicationType>
{
    public ApplicationTypeValueConverter()
        : base(
            e => ApplicationTypeConverter.FromString(e),
            e => ApplicationTypeConverter.ToString(e))
    {
    }
}
