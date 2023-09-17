namespace EtherGizmos.SqlMonitor.Api;

/// <summary>
/// Constants used by the application.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Constants shared by all caches.
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// The suffix to this application's cache lock names.
        /// </summary>
        public const string LockSuffix = "$$lock";

        /// <summary>
        /// The prefix to this application's cache key names.
        /// </summary>
        public const string SchemaName = "sqlpulse";
    }

    /// <summary>
    /// Constants used for RabbitMQ.
    /// </summary>
    public static class RabbitMQ
    {
        /// <summary>
        /// The default RabbitMQ port.
        /// </summary>
        public const short Port = 5672;
    }
}
