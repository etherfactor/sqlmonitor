using EtherGizmos.SqlMonitor.Api.Helpers;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Data.Configuration;
using EtherGizmos.SqlMonitor.Api.Services.Messaging.Configuration;

namespace EtherGizmos.SqlMonitor.Api.Services.Configuration;

public class UsageOptions
{
    public CacheType Cache { get; set; }

    public DatabaseType Database { get; set; }

    public MessageBrokerType MessageBroker { get; set; }

    public void AssertValid(string rootPath)
    {
        if (Cache == CacheType.Unknown)
            ThrowHelper.ForInvalidConfiguration(rootPath, this, e => e.Cache, $"be one of '{string.Join("', '", Enum.GetValues<CacheType>().Where(e => e != CacheType.Unknown))}'");
        if (Database == DatabaseType.Unknown)
            ThrowHelper.ForInvalidConfiguration(rootPath, this, e => e.Database, $"be one of '{string.Join("', '", Enum.GetValues<DatabaseType>().Where(e => e != DatabaseType.Unknown))}'");
        if (MessageBroker == MessageBrokerType.Unknown)
            ThrowHelper.ForInvalidConfiguration(rootPath, this, e => e.MessageBroker, $"be one of '{string.Join("', '", Enum.GetValues<MessageBrokerType>().Where(e => e != MessageBrokerType.Unknown))}'");
    }
}
