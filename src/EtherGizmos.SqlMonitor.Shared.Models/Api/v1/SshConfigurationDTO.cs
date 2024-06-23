using EtherGizmos.SqlMonitor.Shared.Models.Api.v1.Enums;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1;

public class SshConfigurationDTO
{
    public virtual SshAuthenticationTypeDTO? AuthenticationType { get; set; }

    public virtual string? Username { get; set; }

    public virtual string? Password { get; set; }

    public virtual string? PrivateKey { get; set; }

    public virtual string? PrivateKeyPassword { get; set; }
}
