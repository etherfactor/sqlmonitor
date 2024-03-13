namespace EtherGizmos.SqlMonitor.Models.Database.Enums;

public enum SecurableType
{
    Unknown = -1,
    MonitoredSystem = 100,
    MonitoredResource = 125,
    MonitoredEnvironment = 150,
}

public static class SecurableTypeConverter
{
    private static Dictionary<SecurableType, int> Mappings { get; } = new Dictionary<SecurableType, int>()
    {
        { SecurableType.MonitoredSystem, 100 },
        { SecurableType.MonitoredResource, 125 },
        { SecurableType.MonitoredEnvironment, 150 },
    };

    public static SecurableType FromInteger(int value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static SecurableType? FromIntegerOrDefault(int? value)
    {
        if (value == null || !Mappings.ContainsValue((int)value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static int ToInteger(SecurableType value)
    {
        if (value == SecurableType.Unknown)
            return -1;

        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static int? ToIntegerOrDefault(SecurableType? value)
    {
        if (value == SecurableType.Unknown)
            return -1;

        if (value == null || !Mappings.ContainsKey((SecurableType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
