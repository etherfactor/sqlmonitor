namespace EtherGizmos.SqlMonitor.Models.Database.Enums;

public enum SecurableType
{
    Unknown = 0,
    MonitoredSystem = 110,
    MonitoredResource = 120,
    MonitoredEnvironment = 130,
    MonitoredScriptTarget = 150,
    Script = 300,
    ScriptInterpreter = 390,
}

public static class SecurableTypeConverter
{
    private static Dictionary<SecurableType, int> Mappings { get; } = new Dictionary<SecurableType, int>()
    {
        { SecurableType.MonitoredSystem, 110 },
        { SecurableType.MonitoredResource, 120 },
        { SecurableType.MonitoredEnvironment, 130 },
        { SecurableType.MonitoredScriptTarget, 150 },
        { SecurableType.Script, 300 },
        { SecurableType.ScriptInterpreter, 390 },
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
