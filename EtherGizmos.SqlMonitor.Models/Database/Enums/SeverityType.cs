namespace EtherGizmos.SqlMonitor.Models.Database.Enums;

public enum SeverityType
{
    Unknown = -1,
    Nominal = 1,
    Warning = 2,
    Error = 3,
    Critical = 4,
}

public static class SeverityTypeConverter
{
    private static Dictionary<SeverityType, string> Mappings { get; } = new Dictionary<SeverityType, string>()
    {
        { SeverityType.Nominal, "NOMINAL" },
        { SeverityType.Warning, "WARNING" },
        { SeverityType.Error, "ERROR" },
        { SeverityType.Critical, "CRITICAL" },
    };

    public static SeverityType FromString(string value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static SeverityType? FromStringOrDefault(string? value)
    {
        if (value == null || !Mappings.ContainsValue(value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string ToString(SeverityType value)
    {
        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static string? ToStringOrDefault(SeverityType? value)
    {
        if (value == null || !Mappings.ContainsKey((SeverityType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
