using OpenIddict.Abstractions;

namespace EtherGizmos.SqlMonitor.Models.Authorization.Enums;

public enum ClientType
{
    Unknown = 0,
    Public = 10,
    Confidential = 20,
}

public static class ClientTypeConverter
{
    private static Dictionary<ClientType, string?> Mappings { get; } = new Dictionary<ClientType, string?>()
    {
        { ClientType.Public, OpenIddictConstants.ClientTypes.Public },
        { ClientType.Confidential, OpenIddictConstants.ClientTypes.Confidential },
    };

    public static ClientType FromString(string? value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static ClientType? FromStringOrDefault(string? value)
    {
        if (value == null || !Mappings.ContainsValue((string?)value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string? ToString(ClientType value)
    {
        if (value == ClientType.Unknown)
            return null;

        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static string? ToStringOrDefault(ClientType? value)
    {
        if (value == ClientType.Unknown)
            return null;

        if (value == null || !Mappings.ContainsKey((ClientType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
