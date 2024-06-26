﻿using EtherGizmos.SqlMonitor.Shared.Configuration.Helpers;

namespace EtherGizmos.SqlMonitor.Shared.Configuration.Caching;

public class RedisOptions : IValidatableOptions
{
    public string? Username { get; set; }

    public string? Password { get; set; }

    public List<HostAddressOptions> Hosts { get; set; } = new List<HostAddressOptions>();

    public void AssertValid(string rootPath)
    {
        if (Hosts is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, e => e.Hosts);
        if (Hosts.Count == 0)
            ThrowHelper.ForInvalidConfiguration(rootPath, this, e => e.Hosts, "contain at least 1 element");

        for (var i = 0; i < Hosts.Count; i++)
        {
            var host = Hosts[i];
            host.AssertValid(rootPath + $":Hosts:{i}");
        }
    }
}
