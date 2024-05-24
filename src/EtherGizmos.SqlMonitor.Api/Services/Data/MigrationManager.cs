using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using FluentMigrator.Runner;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Manages database migrations.
/// </summary>
public class MigrationManager : IMigrationManager
{
    private readonly object @lock = new();
    private readonly IServiceProvider _serviceProvider;

    private bool _hasRun = false;

    public MigrationManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public void EnsureMigrated()
    {
        if (_hasRun)
            return;

        lock (@lock)
        {
            if (_hasRun)
                return;

            var runner = _serviceProvider.CreateScope()
                .ServiceProvider
                .GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();
            _hasRun = true;
        }
    }
}
