using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Shared.OAuth.Services.ValueConverters;

public class GuidToStringValueConverter : ValueConverter<string, Guid>
{
    public GuidToStringValueConverter()
        : base(
            e => new Guid(e!),
            e => e.ToString())
    {
    }
}

public class NullableGuidToStringValueConverter : ValueConverter<string?, Guid?>
{
    public NullableGuidToStringValueConverter()
        : base(
            e => e != null ? new Guid(e) : null,
            e => e != null ? e.ToString() : null)
    {
    }
}
