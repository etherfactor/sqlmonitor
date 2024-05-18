namespace EtherGizmos.SqlMonitor.Api;

/// <summary>
/// Constants used by the application.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Constants used for OAuth2.0
    /// </summary>
    public static class OAuth2
    {
        private const string RootPath = "/oauth/v2.0";

        public static class Endpoints
        {
            public const string Token = RootPath + "/token";
        }
    }

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

    /// <summary>
    /// Constants used for Redis.
    /// </summary>
    public static class Redis
    {
        /// <summary>
        /// The default Redis port.
        /// </summary>
        public const short Port = 6379;
    }
}
