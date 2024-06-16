namespace EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

public enum SeverityType
{
    Unknown = 0,
    Nominal = 1,
    Warning = 10,
    Error = 20,
    Critical = 30,
}

public static class SeverityTypeConverter
{
    private static Dictionary<SeverityType, int> Mappings { get; } = new Dictionary<SeverityType, int>()
    {
        { SeverityType.Nominal, 1 },
        { SeverityType.Warning, 10 },
        { SeverityType.Error, 20 },
        { SeverityType.Critical, 30 },
    };

    public static SeverityType FromInteger(int value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static SeverityType? FromIntegerOrDefault(int? value)
    {
        if (value == null || !Mappings.ContainsValue((int)value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static int ToInteger(SeverityType value)
    {
        if (value == SeverityType.Unknown)
            return -1;

        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static int? ToIntegerOrDefault(SeverityType? value)
    {
        if (value == SeverityType.Unknown)
            return -1;

        if (value == null || !Mappings.ContainsKey((SeverityType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
