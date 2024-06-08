using EtherGizmos.SqlMonitor.Shared.Messaging;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EtherGizmos.SqlMonitor.Api.Core.Helpers;

internal static class ConnectionTokenHelper
{
    public const string SecurityKey =
        "1Q3V2h3ji3GJtIqntFdelvL88CV97NxU" +
        "lhSD6CVQVZ6WJYj9Ejcusq3XVhQANA6w" +
        "cx3jTOm3cZCi4urBjldPgfZlwu1ORev4" +
        "V4HNSh7MPAnd5QIxN7wed9WH5fDIDdWw" +
        "PwnjpQNlxEdSnpWefc873LrkqilGtqjl" +
        "XYNe6S72TL8NWyo6FjwTGDFtKglG2I0X" +
        "uOhluEFFz1Ye9eJm2flUREN6HsxKXiL4" +
        "8I3ncR8UvE9hcbil3On7HQ2swyfSezzE";

    public static string CreateFor(MonitoredQueryTarget queryTarget, DateTimeOffset expiry)
    {
        var claims = new Dictionary<string, object>()
        {
            { MessagingConstants.Claims.TargetType, "query" },
            { MessagingConstants.Claims.Id, queryTarget.Id },
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));
        var tokenCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

        var tokenDefinition = new SecurityTokenDescriptor()
        {
            Claims = claims,
            Expires = expiry.UtcDateTime,
            SigningCredentials = tokenCredentials,
        };

        var handler = new JsonWebTokenHandler();
        handler.SetDefaultTimesOnTokenCreation = false;

        var token = handler.CreateToken(tokenDefinition);

        return token;
    }

    public static string CreateFor(MonitoredScriptTarget scriptTarget, DateTimeOffset expiry)
    {
        var claims = new Dictionary<string, object>()
        {
            { MessagingConstants.Claims.TargetType, "script" },
            { MessagingConstants.Claims.Id, scriptTarget.Id },
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));
        var tokenCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

        var tokenDefinition = new SecurityTokenDescriptor()
        {
            Claims = claims,
            Expires = expiry.UtcDateTime,
            SigningCredentials = tokenCredentials,
        };

        var handler = new JsonWebTokenHandler();
        handler.SetDefaultTimesOnTokenCreation = false;

        var token = handler.CreateToken(tokenDefinition);

        return token;
    }
}
