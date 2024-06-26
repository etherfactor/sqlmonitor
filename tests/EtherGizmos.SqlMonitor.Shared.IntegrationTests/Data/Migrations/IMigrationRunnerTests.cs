﻿using EtherGizmos.Extensions.DependencyInjection;
using EtherGizmos.SqlMonitor.Shared.Database;
using EtherGizmos.SqlMonitor.Shared.Database.Services;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.SqlMonitor.Shared.IntegrationTests.Data.Migrations;

public abstract class IMigrationRunnerTests : IntegrationTestBase
{
    public abstract string CreateDatabaseCommand { get; }

    public virtual string? PostCreateDatabaseCommand { get; }

    protected abstract IDictionary<string, string?> GetConfigurationValues();

    protected abstract void AddDatabaseConnectionProvider(IServiceCollection serviceCollection);

    protected abstract void AddFluentMigratorDatabaseService(IMigrationRunnerBuilder builder);

    [Test]
    public async Task PerformMigration_MigratesAndReverts()
    {
        var configOptions = GetConfigurationValues();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configOptions)
            .Build();

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IConfiguration>(config);
        AddDatabaseConnectionProvider(serviceCollection);

        serviceCollection
            .AddChildContainer((childServices, parentServices) =>
            {
                var connectionProvider = parentServices.GetRequiredService<IDatabaseConnectionProvider>();

                childServices.AddFluentMigratorCore()
                    .ConfigureRunner(opt =>
                    {
                        AddFluentMigratorDatabaseService(opt);
                        opt.WithGlobalConnectionString(connectionProvider.GetConnectionString())
                            .ScanIn(typeof(DatabaseMigrationTarget).Assembly).For.Migrations()
                            .WithVersionTable(new CustomVersionTableMetadata());
                    })
                    .AddLogging(opt => opt.AddFluentMigratorConsole());
            })
            .ForwardTransient<IMigrationRunner>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var connectionProvider = serviceProvider.GetRequiredService<IDatabaseConnectionProvider>();

        using var connection = connectionProvider.GetDefaultConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();

        //Create a new database for this test
        command.CommandText = CreateDatabaseCommand;
        await command.ExecuteNonQueryAsync();

        if (PostCreateDatabaseCommand is not null)
        {
            using var newConnection = connectionProvider.GetConnection();
            await newConnection.OpenAsync();

            using var newCommand = newConnection.CreateCommand();

            //Run the post-setup command
            newCommand.CommandText = PostCreateDatabaseCommand;
            await newCommand.ExecuteNonQueryAsync();
        }

        //Perform the database migration
        Assert.DoesNotThrow(() =>
        {
            var migrationRunner = serviceProvider.GetRequiredService<IMigrationRunner>();

            //Migrate up to ensure forward migrations work
            migrationRunner.MigrateUp(long.MaxValue);

            //Migrate down to ensure backward migrations work
            migrationRunner.MigrateDown(long.MinValue);

            //Migrate up again to ensure the down migrations clean up everything
            migrationRunner.MigrateUp(long.MaxValue);
        });
    }
}
