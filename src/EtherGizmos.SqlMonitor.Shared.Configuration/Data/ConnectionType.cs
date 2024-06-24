namespace EtherGizmos.SqlMonitor.Shared.Configuration.Data;

public enum ConnectionType
{
    /// <summary>
    /// The type of connection was not specified, or an invalid value was provided.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// An open-source relational database management system developed by Oracle.
    /// </summary>
    MySql = 120,

    /// <summary>
    /// An open-source object-relational database management system.
    /// </summary>
    PostgreSql = 130,

    /// <summary>
    /// A relational database management system developed by Microsoft.
    /// </summary>
    SqlServer = 110,

    RabbitMq = 210,

    Redis = 310,
}
