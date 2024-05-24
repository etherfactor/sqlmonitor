using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Database;
using FluentMigrator.Runner;
using Serilog;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides means for performing migrations against the database. Note: creates a separate service collection for Fluent
/// Migrator, as Fluent Migrator services cannot reference other services when added. This leads to issues when performing
/// integration tests, unless done this way.
/// </summary>
public static class DatabaseMigrationRunner
{
    /// <summary>
    /// Perform the migration, given a specific database connection.
    /// </summary>
    /// <param name="connectionProvider">The database connection to utilize.</param>
    /// <param name="version">The target version.</param>
    public static void PerformMigration(IDatabaseConnectionProvider connectionProvider, long version = long.MaxValue)
    {
        //Perform the database migration
        var runner = GetRunner(connectionProvider);
        runner.MigrateUp(version);
    }

    /// <summary>
    /// Revert the migration, given a specific database connection.
    /// </summary>
    /// <param name="connectionProvider"></param>
    /// <param name="version">The target version.</param>
    public static void RevertMigration(IDatabaseConnectionProvider connectionProvider, long version = 0)
    {
        //Perform the database migration
        var runner = GetRunner(connectionProvider);
        runner.MigrateDown(version);
    }

    /// <summary>
    /// Constructs a database migration runner for the provided connection.
    /// </summary>
    /// <param name="connectionProvider">The database connection to utilize.</param>
    /// <returns>The constructed runner.</returns>
    private static IMigrationRunner GetRunner(IDatabaseConnectionProvider connectionProvider)
    {
        var migrationCollection = new ServiceCollection()
            .AddFluentMigratorCore()
            .AddLogging(opt =>
            {
                opt.AddSerilog(Log.Logger);
            })
            .ConfigureRunner(opt =>
            {
                opt.AddSqlServer2016()
                    .WithGlobalConnectionString(connectionProvider.GetConnectionString())
                    .ScanIn(typeof(DatabaseMigrationTarget).Assembly).For.Migrations()
                    .WithVersionTable(new CustomVersionTableMetadata());
            });

        var migrationProvider = migrationCollection
            .BuildServiceProvider()
            .CreateScope()
            .ServiceProvider;

        //Create the runner
        var runner = migrationProvider.GetRequiredService<IMigrationRunner>();
        return runner;
    }
}
