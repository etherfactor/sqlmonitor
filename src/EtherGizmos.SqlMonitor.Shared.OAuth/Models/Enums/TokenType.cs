﻿using OpenIddict.Abstractions;

namespace EtherGizmos.SqlMonitor.Shared.OAuth.Models.Enums;

public enum TokenType
{
    Unknown = 0,
    Bearer = 10,
    AccessToken = 20,
    IdentityToken = 25,
    RefreshToken = 30,
}

public static class TokenTypeConverter
{
    private static Dictionary<TokenType, string?> Mappings { get; } = new Dictionary<TokenType, string?>()
    {
        { TokenType.Bearer, OpenIddictConstants.TokenTypes.Bearer },
        { TokenType.AccessToken, "access_token" },
        { TokenType.IdentityToken, "id_token" },
        { TokenType.RefreshToken, "refresh_token" },
    };

    public static TokenType FromString(string? value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static TokenType? FromStringOrDefault(string? value)
    {
        if (value == null || !Mappings.ContainsValue((string?)value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string? ToString(TokenType value)
    {
        if (value == TokenType.Unknown)
            return null;

        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static string? ToStringOrDefault(TokenType? value)
    {
        if (value == TokenType.Unknown)
            return null;

        if (value == null || !Mappings.ContainsKey((TokenType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
