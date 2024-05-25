namespace EtherGizmos.SqlMonitor.Shared.Models.Database.Enums;

public enum AgentType
{
    Unknown = 0,
    Distributed = 10,
    Dedicated = 20,
}

public static class AgentTypeConverter
{
    private static Dictionary<AgentType, int> Mappings { get; } = new Dictionary<AgentType, int>()
    {
        { AgentType.Distributed, 10 },
        { AgentType.Dedicated, 20 },
    };

    public static AgentType FromInteger(int value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static AgentType? FromIntegerOrDefault(int? value)
    {
        if (value == null || !Mappings.ContainsValue((int)value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static int ToInteger(AgentType value)
    {
        if (value == AgentType.Unknown)
            return -1;

        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static int? ToIntegerOrDefault(AgentType? value)
    {
        if (value == AgentType.Unknown)
            return -1;

        if (value == null || !Mappings.ContainsKey((AgentType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
