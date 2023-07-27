namespace EtherGizmos.SqlMonitor.Models.Database.Enums;

public enum PrincipalType
{
    Unknown = -1,
    User = 1
}

public static class PrincipalTypeConverter
{
    private static Dictionary<PrincipalType, string> Mappings { get; } = new Dictionary<PrincipalType, string>()
    {
        { PrincipalType.User, "USER" }
    };

    public static PrincipalType FromString(string value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static PrincipalType? FromStringOrDefault(string? value)
    {
        if (value == null || !Mappings.ContainsValue(value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string ToString(PrincipalType value)
    {
        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static string? ToStringOrDefault(PrincipalType? value)
    {
        if (value == null || !Mappings.ContainsKey((PrincipalType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
