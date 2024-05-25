using EtherGizmos.SqlMonitor.Api.Services.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

namespace EtherGizmos.SqlMonitor.Api.Controllers.Authorization;

public class OAuth2Controller : ControllerBase
{
    private readonly ILogger _logger;
    private readonly ApplicationContext _context;
    private readonly IOpenIddictApplicationManager _applicationManager;

    public OAuth2Controller(
        ILogger<OAuth2Controller> logger,
        ApplicationContext context,
        IOpenIddictApplicationManager applicationManager)
    {
        _logger = logger;
        _context = context;
        _applicationManager = applicationManager;
    }

    [HttpPost(Constants.OAuth2.Endpoints.Token)]
    public async Task<IActionResult> Token()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException();

        var clientId = request.ClientId
            ?? throw new InvalidOperationException();

        if (request.IsClientCredentialsGrantType())
        {
            var application = await _applicationManager.FindByClientIdAsync(clientId)
                ?? throw new InvalidOperationException();

            var applicationId = int.Parse(await _applicationManager.GetIdAsync(application) ?? throw new InvalidOperationException());
            var maybeAgent = await _context.Agents.SingleOrDefaultAsync(e => e.ApplicationId == applicationId);

            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: OpenIddictConstants.Claims.Name,
                roleType: OpenIddictConstants.Claims.Role);

            identity.SetClaim(OpenIddictConstants.Claims.Name, await _applicationManager.GetDisplayNameAsync(application));

            if (maybeAgent is not null)
            {
                identity.SetClaim(OpenIddictConstants.Claims.Subject, maybeAgent.Id.ToString());
                identity.SetClaim("type", "agent");
            }

            identity.SetScopes(request.GetScopes());
            identity.SetDestinations(GetDestinations);

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new NotImplementedException();
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        return claim.Type switch
        {
            OpenIddictConstants.Claims.Name or OpenIddictConstants.Claims.Subject => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],
            _ => [OpenIddictConstants.Destinations.AccessToken],
        };
    }
}
