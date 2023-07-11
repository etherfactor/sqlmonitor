using EtherGizmos.SqlMonitor.Api.Data.Access;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using EtherGizmos.SqlMonitor.Database;
using FluentMigrator.Runner;

namespace EtherGizmos.SqlMonitor.Api.Data.Migrations;

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
    public static void PerformMigration(IDatabaseConnectionProvider connectionProvider)
    {
        var migrationCollection = new ServiceCollection()
            .AddFluentMigratorCore()
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

        //Perform the database migration
        var runner = migrationProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}
