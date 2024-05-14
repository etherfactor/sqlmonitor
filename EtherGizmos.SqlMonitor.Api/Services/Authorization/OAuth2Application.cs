using OpenIddict.EntityFrameworkCore.Models;

namespace EtherGizmos.SqlMonitor.Api.Services.Authorization;

public class OAuth2Application : OpenIddictEntityFrameworkCoreApplication<int, OAuth2Authorization, OAuth2Token>
{
}
