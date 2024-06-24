namespace EtherGizmos.SqlMonitor.Shared.OAuth;

/// <summary>
/// Constants used for OAuth2.0.
/// </summary>
public static class OAuthConstants
{
    private const string RootPath = "/oauth/v2.0";

    public static class Endpoints
    {
        public const string Token = RootPath + "/token";
    }
}
