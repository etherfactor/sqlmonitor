using EtherGizmos.SqlMonitor.Api.Services.Caching.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Data.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Messaging.Configuration;

namespace EtherGizmos.SqlMonitor.Api.Services.Configuration;

public class UsageOptions
{
    public CacheType Cache { get; set; }

    public DatabaseType Database { get; set; }

    public MessageBrokerType MessageBroker { get; set; }
}
