using OpenIddict.EntityFrameworkCore.Models;

namespace EtherGizmos.SqlMonitor.Api.Services.Authorization;

public class OAuth2Token : OpenIddictEntityFrameworkCoreToken<int, OAuth2Application, OAuth2Authorization>
{
}
