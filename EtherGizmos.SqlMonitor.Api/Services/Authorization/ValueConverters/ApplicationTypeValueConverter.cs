using EtherGizmos.SqlMonitor.Api.Services.Authorization.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Api.Services.Authorization.ValueConverters;

public class ApplicationTypeValueConverter : ValueConverter<string?, int>
{
    public ApplicationTypeValueConverter()
        : base(e => (int)ApplicationTypeConverter.FromString(e), e => ApplicationTypeConverter.ToString((ApplicationType)e))
    {
    }
}
