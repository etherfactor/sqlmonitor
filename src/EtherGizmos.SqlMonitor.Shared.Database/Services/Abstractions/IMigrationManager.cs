namespace EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;

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
