namespace EtherGizmos.SqlMonitor.Models.Database.Enums;

public enum AggregateType
{
    Unknown = -1,
    Average = 1,
    Maximum = 2,
    Minimum = 3,
    StandardDeviation = 4,
    Sum = 5,
    Variance = 6,
}

public static class AggregateTypeConverter
{
    private static Dictionary<AggregateType, string> Mappings { get; } = new Dictionary<AggregateType, string>()
    {
        { AggregateType.Average, "AVERAGE" },
        { AggregateType.Maximum, "MAXIMUM" },
        { AggregateType.Minimum, "MINIMUM" },
        { AggregateType.StandardDeviation, "STD_DEV" },
        { AggregateType.Sum, "SUM" },
        { AggregateType.Variance, "VARIANCE" },
    };

    public static AggregateType FromString(string value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static AggregateType? FromStringOrDefault(string? value)
    {
        if (value == null || !Mappings.ContainsValue(value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string ToString(AggregateType value)
    {
        if (value == AggregateType.Unknown)
            return "UNKNOWN";

        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static string? ToStringOrDefault(AggregateType? value)
    {
        if (value == AggregateType.Unknown)
            return "UNKNOWN";

        if (value == null || !Mappings.ContainsKey((AggregateType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
