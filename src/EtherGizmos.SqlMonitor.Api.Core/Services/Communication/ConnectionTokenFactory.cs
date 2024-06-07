using EtherGizmos.SqlMonitor.Api.Core.Services.Communication.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Messaging;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EtherGizmos.SqlMonitor.Api.Core.Services.Communication;

internal class ConnectionTokenFactory : IConnectionTokenFactory
{
    public string CreateFor(MonitoredQueryTarget queryTarget, DateTimeOffset expiry)
    {
        var claims = new List<Claim>
        {
            new(MessagingConstants.Claims.Id, queryTarget.Id.ToString()),
        };

        var token = CreateToken(claims, expiry);
        return token;
    }

    public string CreateFor(MonitoredScriptTarget scriptTarget, DateTimeOffset expiry)
    {
        var claims = new List<Claim>
        {
            new(MessagingConstants.Claims.Id, scriptTarget.Id.ToString()),
        };

        var token = CreateToken(claims, expiry);
        return token;
    }

    private string CreateToken(List<Claim> claims, DateTimeOffset expiry)
    {
        var tokenCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abcdef")), SecurityAlgorithms.HmacSha512);

        var tokenDefinition = new JwtSecurityToken(
        claims: claims,
            expires: expiry.UtcDateTime,
            signingCredentials: tokenCredentials);

        var token = new JwtSecurityTokenHandler()
            .WriteToken(tokenDefinition);

        return token;
    }
}
