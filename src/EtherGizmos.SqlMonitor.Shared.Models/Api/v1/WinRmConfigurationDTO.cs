using EtherGizmos.SqlMonitor.Shared.Models.Api.v1.Enums;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1;

public class WinRmConfigurationDTO
{
    public virtual WinRmAuthenticationTypeDTO? AuthenticationType { get; set; }

    public virtual bool? UseSsl { get; set; }

    public virtual string? Username { get; set; }

    public virtual string? Password { get; set; }
}
