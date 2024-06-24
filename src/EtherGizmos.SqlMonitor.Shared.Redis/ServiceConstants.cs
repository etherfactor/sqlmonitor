namespace EtherGizmos.SqlMonitor.Shared.Redis;

/// <summary>
/// Constants used for application services.
/// </summary>
public static class ServiceConstants
{
    /// <summary>
    /// Constants shared by all caches.
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// The prefix to this application's cache lock names.
        /// </summary>
        public const string LockPrefix = "$$lock";

        /// <summary>
        /// The prefix to this application's cache key names.
        /// </summary>
        public const string SchemaName = "performancepulse";
    }
}
