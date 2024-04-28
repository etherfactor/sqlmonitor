namespace EtherGizmos.SqlMonitor.Models.Database.Enums;

public enum WinRmAuthenticationType
{
    Unknown = 0,
    Kerberos = 10,
    Basic = 20,
}

public static class WinRmAuthenticationTypeConverter
{
    private static Dictionary<WinRmAuthenticationType, int> Mappings { get; } = new Dictionary<WinRmAuthenticationType, int>()
    {
        { WinRmAuthenticationType.Kerberos, 10 },
        { WinRmAuthenticationType.Basic, 20 },
    };

    public static WinRmAuthenticationType FromInteger(int value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static WinRmAuthenticationType? FromIntegerOrDefault(int? value)
    {
        if (value == null || !Mappings.ContainsValue((int)value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static int ToInteger(WinRmAuthenticationType value)
    {
        if (value == WinRmAuthenticationType.Unknown)
            return -1;

        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static int? ToIntegerOrDefault(WinRmAuthenticationType? value)
    {
        if (value == WinRmAuthenticationType.Unknown)
            return -1;

        if (value == null || !Mappings.ContainsKey((WinRmAuthenticationType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
