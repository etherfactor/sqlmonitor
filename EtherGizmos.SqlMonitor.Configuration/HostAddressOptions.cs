using EtherGizmos.SqlMonitor.Configuration.Helpers;

namespace EtherGizmos.SqlMonitor.Configuration;

public class HostAddressOptions
{
    public string Address { get; set; } = null!;

    public short Port { get; set; }

    public void AssertValid(string rootPath)
    {
        if (Address is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, e => e.Address);
        if (Port <= 0)
            ThrowHelper.ForInvalidConfiguration(rootPath, this, e => e.Port, "be at least 1");
    }
}
