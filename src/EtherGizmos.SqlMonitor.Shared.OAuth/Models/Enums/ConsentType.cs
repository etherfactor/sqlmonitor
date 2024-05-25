using OpenIddict.Abstractions;

namespace EtherGizmos.SqlMonitor.Shared.OAuth.Models.Enums;

public enum ConsentType
{
    Unknown = 0,
    Explicit = 10,
    External = 20,
    Implicit = 30,
    Systematic = 40,
}

public static class ConsentTypeConverter
{
    private static Dictionary<ConsentType, string?> Mappings { get; } = new Dictionary<ConsentType, string?>()
    {
        { ConsentType.Explicit, OpenIddictConstants.ConsentTypes.Explicit },
        { ConsentType.External, OpenIddictConstants.ConsentTypes.External },
        { ConsentType.Implicit, OpenIddictConstants.ConsentTypes.Implicit },
        { ConsentType.Systematic, OpenIddictConstants.ConsentTypes.Systematic },
    };

    public static ConsentType FromString(string? value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static ConsentType? FromStringOrDefault(string? value)
    {
        if (value == null || !Mappings.ContainsValue((string?)value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string? ToString(ConsentType value)
    {
        if (value == ConsentType.Unknown)
            return null;

        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static string? ToStringOrDefault(ConsentType? value)
    {
        if (value == ConsentType.Unknown)
            return null;

        if (value == null || !Mappings.ContainsKey((ConsentType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
