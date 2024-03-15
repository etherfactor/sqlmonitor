using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides access to database tables.
/// </summary>
[ExcludeFromCodeCoverage]
public class DatabaseContext : DbContext
{
    /// <summary>
    /// Provides access to <see cref="MonitoredEnvironment"/> records, in 'dbo.monitored_environments'.
    /// </summary>
    public virtual DbSet<MonitoredEnvironment> MonitoredEnvironments { get; set; }

    /// <summary>
    /// Provides access to <see cref="MonitoredResource"/> records, in 'dbo.monitored_resources'.
    /// </summary>
    public virtual DbSet<MonitoredResource> MonitoredResources { get; set; }

    /// <summary>
    /// Provides access to <see cref="MonitoredSystem"/> records, in 'dbo.monitored_systems'.
    /// </summary>
    public virtual DbSet<MonitoredSystem> MonitoredSystems { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseContext"/> class using the specified options. The
    /// <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/> will still be called to allow further configuration
    /// of the options.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    /// <param name="migrationManager">Manages database migrations.</param>
    public DatabaseContext(DbContextOptions options, IMigrationManager migrationManager) : base(options)
    {
        migrationManager.EnsureMigrated();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseContext"/> class. The
    /// <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/> method will be called to configure the database (and
    /// other options) to be used for this context.
    /// </summary>
    /// <param name="migrationManager">Manages database migrations.</param>
    protected DatabaseContext(IMigrationManager migrationManager) : base()
    {
        migrationManager.EnsureMigrated();
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //**********************************************************
        // Add Entities

        modelBuilder.Entity<MonitoredEnvironment>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);
            entity.PropertyWithAnnotations(e => e.IsSoftDeleted);
            entity.PropertyWithAnnotations(e => e.SecurableId);
        });

        modelBuilder.Entity<MonitoredResource>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);
            entity.PropertyWithAnnotations(e => e.IsSoftDeleted);
            entity.PropertyWithAnnotations(e => e.SecurableId);
        });

        modelBuilder.Entity<MonitoredSystem>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);
            entity.PropertyWithAnnotations(e => e.IsSoftDeleted);
            entity.PropertyWithAnnotations(e => e.SecurableId);
        });

        modelBuilder.Entity<Securable>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.PropertyWithAnnotations(e => e.Type);
        });

        //**********************************************************
        // Add Value Converters

        modelBuilder.AddGlobalValueConverter(new ValueConverter<DateTimeOffset, DateTime>(
            app => app.UtcDateTime,
            db => new DateTimeOffset(db, TimeSpan.Zero)));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<DateTimeOffset?, DateTime?>(
            app => app != null ? app.Value.UtcDateTime : null,
            db => db != null ? new DateTimeOffset((DateTime)db, TimeSpan.Zero) : null));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<SecurableType, int>(
            app => SecurableTypeConverter.ToInteger(app),
            db => SecurableTypeConverter.FromInteger(db)));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<SecurableType?, int?>(
            app => SecurableTypeConverter.ToIntegerOrDefault(app),
            db => SecurableTypeConverter.FromIntegerOrDefault(db)));
    }
}
