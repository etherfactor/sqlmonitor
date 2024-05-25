namespace EtherGizmos.SqlMonitor.Shared.Models.Authorization.Enums;

public enum StatusType
{
    Unknown = 0,
    Valid = 10,
    Inactive = 20,
    Redeemed = 30,
    Rejected = 40,
    Revoked = 50,
}

public static class StatusTypeConverter
{
    private static Dictionary<StatusType, string?> Mappings { get; } = new Dictionary<StatusType, string?>()
    {
        { StatusType.Valid, OpenIddictConstants.Statuses.Valid },
        { StatusType.Inactive, OpenIddictConstants.Statuses.Inactive },
        { StatusType.Redeemed, OpenIddictConstants.Statuses.Redeemed },
        { StatusType.Rejected, OpenIddictConstants.Statuses.Rejected },
        { StatusType.Revoked, OpenIddictConstants.Statuses.Revoked },
    };

    public static StatusType FromString(string? value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static StatusType? FromStringOrDefault(string? value)
    {
        if (value == null || !Mappings.ContainsValue((string?)value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string? ToString(StatusType value)
    {
        if (value == StatusType.Unknown)
            return null;

        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static string? ToStringOrDefault(StatusType? value)
    {
        if (value == StatusType.Unknown)
            return null;

        if (value == null || !Mappings.ContainsKey((StatusType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
