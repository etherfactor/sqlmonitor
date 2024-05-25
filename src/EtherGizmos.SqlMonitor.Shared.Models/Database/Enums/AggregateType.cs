namespace EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

public enum AggregateType
{
    Unknown = 0,
    Sum = 10,
    Average = 20,
    Minimum = 30,
    Maximum = 35,
}

public static class AggregateTypeConverter
{
    private static Dictionary<AggregateType, int> Mappings { get; } = new Dictionary<AggregateType, int>()
    {
        { AggregateType.Sum, 10 },
        { AggregateType.Average, 20 },
        { AggregateType.Minimum, 30 },
        { AggregateType.Maximum, 35 },
    };

    public static AggregateType FromInteger(int value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static AggregateType? FromIntegerOrDefault(int? value)
    {
        if (value == null || !Mappings.ContainsValue((int)value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static int ToInteger(AggregateType value)
    {
        if (value == AggregateType.Unknown)
            return -1;

        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static int? ToIntegerOrDefault(AggregateType? value)
    {
        if (value == AggregateType.Unknown)
            return -1;

        if (value == null || !Mappings.ContainsKey((AggregateType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
