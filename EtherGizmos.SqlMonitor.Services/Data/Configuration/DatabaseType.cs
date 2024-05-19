namespace EtherGizmos.SqlMonitor.Services.Data.Configuration;

/// <summary>
/// The type of database to use with the application.
/// </summary>
public enum DatabaseType
{
    /// <summary>
    /// The type of database was not specified, or an invalid value was provided.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// An open-source relational database management system developed by Oracle.
    /// </summary>
    MySql = 20,

    /// <summary>
    /// An open-source object-relational database management system.
    /// </summary>
    PostgreSql = 30,

    /// <summary>
    /// A relational database management system developed by Microsoft.
    /// </summary>
    SqlServer = 10,
}
