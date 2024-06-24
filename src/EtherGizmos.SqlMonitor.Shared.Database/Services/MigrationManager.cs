using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services;

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
