using OpenIddict.Abstractions;

namespace EtherGizmos.SqlMonitor.Models.Authorization.Enums;

public enum ApplicationType
{
    Unknown = 0,
    Native = 10,
    Web = 20,
    Agent = 100,
}

public static class ApplicationTypeConverter
{
    private static Dictionary<ApplicationType, string?> Mappings { get; } = new Dictionary<ApplicationType, string?>()
    {
        { ApplicationType.Native, OpenIddictConstants.ApplicationTypes.Native },
        { ApplicationType.Web, OpenIddictConstants.ApplicationTypes.Web },
        { ApplicationType.Agent, "agent" },
    };

    public static ApplicationType FromString(string? value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static ApplicationType? FromStringOrDefault(string? value)
    {
        if (value == null || !Mappings.ContainsValue((string?)value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string? ToString(ApplicationType value)
    {
        if (value == ApplicationType.Unknown)
            return null;

        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static string? ToStringOrDefault(ApplicationType? value)
    {
        if (value == ApplicationType.Unknown)
            return null;

        if (value == null || !Mappings.ContainsKey((ApplicationType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
