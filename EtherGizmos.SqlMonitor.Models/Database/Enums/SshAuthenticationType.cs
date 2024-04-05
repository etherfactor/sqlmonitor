namespace EtherGizmos.SqlMonitor.Models.Database.Enums;

public enum SshAuthenticationType
{
    Unknown = -1,
    None = 1,
    Password = 10,
    PrivateKey = 20,
}

public static class SshAuthenticationTypeConverter
{
    private static Dictionary<SshAuthenticationType, int> Mappings { get; } = new Dictionary<SshAuthenticationType, int>()
    {
        { SshAuthenticationType.None, 1 },
        { SshAuthenticationType.Password, 10 },
        { SshAuthenticationType.PrivateKey, 20 },
    };

    public static SshAuthenticationType FromInteger(int value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static SshAuthenticationType? FromIntegerOrDefault(int? value)
    {
        if (value == null || !Mappings.ContainsValue((int)value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static int ToInteger(SshAuthenticationType value)
    {
        if (value == SshAuthenticationType.Unknown)
            return -1;

        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static int? ToIntegerOrDefault(SshAuthenticationType? value)
    {
        if (value == SshAuthenticationType.Unknown)
            return -1;

        if (value == null || !Mappings.ContainsKey((SshAuthenticationType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
