using System.Data.Common;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;

/// <summary>
/// Provides a database connection string.
/// </summary>
public interface IDatabaseConnectionProvider
{
    /// <summary>
    /// Get a database connection string.
    /// </summary>
    /// <returns>The connection string.</returns>
    string GetConnectionString();

    /// <summary>
    /// Gets a database connection, using the contained connection string.
    /// </summary>
    /// <returns>The database connection.</returns>
    DbConnection GetConnection();

    /// <summary>
    /// Gets a database connection, opening the default database for the contained connection string.
    /// </summary>
    /// <returns>The default database connection.</returns>
    DbConnection GetDefaultConnection();
}
