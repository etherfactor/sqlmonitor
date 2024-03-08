namespace EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;

/// <summary>
/// Manages database migrations.
/// </summary>
public interface IMigrationManager
{
    /// <summary>
    /// Ensures database migrations have been run, if they have not already.
    /// </summary>
    void EnsureMigrated();
}
