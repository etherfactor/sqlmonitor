namespace EtherGizmos.SqlMonitor.Shared.Models.Authorization.Enums;

public enum AuthorizationType
{
    Unknown = 0,
    Permanent = 10,
    AdHoc = 20,
}

public static class AuthorizationTypeConverter
{
    private static Dictionary<AuthorizationType, string?> Mappings { get; } = new Dictionary<AuthorizationType, string?>()
    {
        { AuthorizationType.Permanent, OpenIddictConstants.AuthorizationTypes.Permanent },
        { AuthorizationType.AdHoc, OpenIddictConstants.AuthorizationTypes.AdHoc },
    };

    public static AuthorizationType FromString(string? value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static AuthorizationType? FromStringOrDefault(string? value)
    {
        if (value == null || !Mappings.ContainsValue((string?)value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string? ToString(AuthorizationType value)
    {
        if (value == AuthorizationType.Unknown)
            return null;

        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static string? ToStringOrDefault(AuthorizationType? value)
    {
        if (value == AuthorizationType.Unknown)
            return null;

        if (value == null || !Mappings.ContainsKey((AuthorizationType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
