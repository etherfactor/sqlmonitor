using EtherGizmos.SqlMonitor.Services.Configuration;
using EtherGizmos.SqlMonitor.Services.Helpers;

namespace EtherGizmos.SqlMonitor.Services.Messaging.Configuration;

public class RabbitMQOptions
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public List<HostAddressOptions> Hosts { get; set; } = new List<HostAddressOptions>();

    public void AssertValid(string rootPath)
    {
        if (Username is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, e => e.Username);
        if (Password is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, e => e.Password);
        if (Hosts is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, e => e.Hosts);
        if (Hosts.Count == 0)
            ThrowHelper.ForInvalidConfiguration(rootPath, this, e => e.Hosts, "contain at least one element");

        for (var i = 0; i < Hosts.Count; i++)
        {
            var host = Hosts[i];
            host.AssertValid(rootPath + $":Hosts:{i}");
        }
    }
}
