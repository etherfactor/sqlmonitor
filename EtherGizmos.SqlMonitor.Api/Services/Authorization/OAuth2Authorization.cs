using OpenIddict.EntityFrameworkCore.Models;

namespace EtherGizmos.SqlMonitor.Api.Services.Authorization;

public class OAuth2Authorization : OpenIddictEntityFrameworkCoreAuthorization<int, OAuth2Application, OAuth2Token>
{
}
