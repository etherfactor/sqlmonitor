namespace EtherGizmos.SqlMonitor.Api.Services.Abstractions;

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
}
