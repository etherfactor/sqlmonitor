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
    /// Provides access to <see cref="Metric"/> records, in 'dbo.metrics'.
    /// </summary>
    public virtual DbSet<Metric> Metrics { get; set; }

    /// <summary>
    /// Provides access to <see cref="MonitoredEnvironment"/> records, in 'dbo.monitored_environments'.
    /// </summary>
    public virtual DbSet<MonitoredEnvironment> MonitoredEnvironments { get; set; }

    /// <summary>
    /// Provides access to <see cref="MonitoredResource"/> records, in 'dbo.monitored_resources'.
    /// </summary>
    public virtual DbSet<MonitoredResource> MonitoredResources { get; set; }

    /// <summary>
    /// Provides access to <see cref="MonitoredScriptTarget"/> records, in 'dbo.monitored_script_targets'.
    /// </summary>
    public virtual DbSet<MonitoredScriptTarget> MonitoredScriptTargets { get; set; }

    /// <summary>
    /// Provides access to <see cref="MonitoredSystem"/> records, in 'dbo.monitored_systems'.
    /// </summary>
    public virtual DbSet<MonitoredSystem> MonitoredSystems { get; set; }

    /// <summary>
    /// Provides access to <see cref="MonitoredTarget"/> records, in 'dbo.monitored_targets'.
    /// </summary>
    public virtual DbSet<MonitoredTarget> MonitoredTargets { get; set; }

    /// <summary>
    /// Provides access to <see cref="Query"/> records, in 'dbo.queries'.
    /// </summary>
    public virtual DbSet<Query> Queries { get; set; }

    /// <summary>
    /// Provides access to <see cref="Script"/> records, in 'dbo.scripts'.
    /// </summary>
    public virtual DbSet<Script> Scripts { get; set; }

    /// <summary>
    /// Provides access to <see cref="ScriptInterpreter"/> records, in 'dbo.script_interpreters'.
    /// </summary>
    public virtual DbSet<ScriptInterpreter> ScriptInterpreters { get; set; }

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

        modelBuilder.Entity<Metric>(entity =>
        {
            entity.ToTableWithAnnotations(buildAction: e =>
            {
                e.HasTrigger("TR_metrics_audit");
            });

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);
            entity.PropertyWithAnnotations(e => e.AggregateType);
            entity.PropertyWithAnnotations(e => e.IsSoftDeleted);
            entity.PropertyWithAnnotations(e => e.SecurableId)
                .HasDefaultValueSql();
        });

        modelBuilder.Entity<MonitoredEnvironment>(entity =>
        {
            entity.ToTableWithAnnotations(buildAction: e =>
            {
                e.HasTrigger("TR_monitored_environments_audit");
            });

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);
            entity.PropertyWithAnnotations(e => e.IsSoftDeleted);
            entity.PropertyWithAnnotations(e => e.SecurableId)
                .HasDefaultValueSql();
        });

        modelBuilder.Entity<MonitoredResource>(entity =>
        {
            entity.ToTableWithAnnotations(buildAction: e =>
            {
                e.HasTrigger("TR_monitored_resources_audit");
            });

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);
            entity.PropertyWithAnnotations(e => e.IsSoftDeleted);
            entity.PropertyWithAnnotations(e => e.SecurableId)
                .HasDefaultValueSql();
        });

        modelBuilder.Entity<MonitoredScriptTarget>(entity =>
        {
            entity.ToTableWithAnnotations(buildAction: e =>
            {
                e.HasTrigger("TR_monitored_script_targets_audit");
            });

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.MonitoredTargetId);
            entity.PropertyWithAnnotations(e => e.ScriptInterpreterId);
            entity.PropertyWithAnnotations(e => e.HostName);
            entity.PropertyWithAnnotations(e => e.RunInPath);
            entity.PropertyWithAnnotations(e => e.SecurableId)
                .HasDefaultValueSql();
        });

        modelBuilder.Entity<MonitoredSystem>(entity =>
        {
            entity.ToTableWithAnnotations(buildAction: e =>
            {
                e.HasTrigger("TR_monitored_systems_audit");
            });

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);
            entity.PropertyWithAnnotations(e => e.IsSoftDeleted);
            entity.PropertyWithAnnotations(e => e.SecurableId)
                .HasDefaultValueSql();
        });

        modelBuilder.Entity<MonitoredTarget>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.PropertyWithAnnotations(e => e.MonitoredSystemId);
            entity.PropertyWithAnnotations(e => e.MonitoredResourceId);
            entity.PropertyWithAnnotations(e => e.MonitoredEnvironmentId);
        });

        modelBuilder.Entity<Query>(entity =>
        {
            entity.ToTableWithAnnotations(buildAction: e =>
            {
                e.HasTrigger("TR_queries_audit");
            });

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);
            entity.PropertyWithAnnotations(e => e.RunFrequency);
            entity.PropertyWithAnnotations(e => e.LastRunAtUtc);
            entity.PropertyWithAnnotations(e => e.IsActive);
            entity.PropertyWithAnnotations(e => e.IsSoftDeleted);
            entity.PropertyWithAnnotations(e => e.BucketColumn);
            entity.PropertyWithAnnotations(e => e.TimestampUtcColumn);
            entity.PropertyWithAnnotations(e => e.SecurableId)
                .HasDefaultValueSql();

            entity.HasMany(e => e.Variants)
                .WithOne(e => e.Query);

            entity.HasMany(e => e.Metrics)
                .WithOne(e => e.Query);
        });

        modelBuilder.Entity<QueryMetric>(entity =>
        {
            entity.ToTableWithAnnotations(buildAction: e =>
            {
                e.HasTrigger("TR_query_metrics_audit");
            });

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.QueryId);
            entity.PropertyWithAnnotations(e => e.MetricId);
            entity.PropertyWithAnnotations(e => e.ValueColumn);
            entity.PropertyWithAnnotations(e => e.IsActive);

            entity.HasOne(e => e.Query)
                .WithMany(e => e.Metrics);
        });

        modelBuilder.Entity<QueryVariant>(entity =>
        {
            entity.ToTableWithAnnotations(buildAction: e =>
            {
                e.HasTrigger("TR_query_variants_audit");
            });

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.QueryId);
            entity.PropertyWithAnnotations(e => e.SqlType);
            entity.PropertyWithAnnotations(e => e.QueryText);

            entity.HasOne(e => e.Query)
                .WithMany(e => e.Variants);
        });

        modelBuilder.Entity<QueryMetric>(entity =>
        {
            entity.ToTableWithAnnotations(buildAction: e =>
            {
                e.HasTrigger("TR_query_metrics_audit");
            });

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.QueryId);
            entity.PropertyWithAnnotations(e => e.MetricId);
            entity.PropertyWithAnnotations(e => e.ValueColumn);
            entity.PropertyWithAnnotations(e => e.IsActive);

            entity.HasOne(e => e.Query)
                .WithMany(e => e.Metrics);
        });

        modelBuilder.Entity<Script>(entity =>
        {
            entity.ToTableWithAnnotations(buildAction: e =>
            {
                e.HasTrigger("TR_scripts_audit");
            });

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);
            entity.PropertyWithAnnotations(e => e.RunFrequency);
            entity.PropertyWithAnnotations(e => e.LastRunAtUtc);
            entity.PropertyWithAnnotations(e => e.IsActive);
            entity.PropertyWithAnnotations(e => e.IsSoftDeleted);
            entity.PropertyWithAnnotations(e => e.BucketKey);
            entity.PropertyWithAnnotations(e => e.TimestampUtcKey);
            entity.PropertyWithAnnotations(e => e.SecurableId)
                .HasDefaultValueSql();

            entity.HasMany(e => e.Variants)
                .WithOne(e => e.Script);

            entity.HasMany(e => e.Metrics)
                .WithOne(e => e.Script);
        });

        modelBuilder.Entity<ScriptInterpreter>(entity =>
        {
            entity.ToTableWithAnnotations(buildAction: e =>
            {
                e.HasTrigger("TR_script_interpreters_audit");
            });

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);
            entity.PropertyWithAnnotations(e => e.Command);
            entity.PropertyWithAnnotations(e => e.Arguments);
            entity.PropertyWithAnnotations(e => e.IsSoftDeleted);
            entity.PropertyWithAnnotations(e => e.SecurableId)
                .HasDefaultValueSql();
        });

        modelBuilder.Entity<ScriptMetric>(entity =>
        {
            entity.ToTableWithAnnotations(buildAction: e =>
            {
                e.HasTrigger("TR_script_metrics_audit");
            });

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.ScriptId);
            entity.PropertyWithAnnotations(e => e.MetricId);
            entity.PropertyWithAnnotations(e => e.ValueKey);
            entity.PropertyWithAnnotations(e => e.IsActive);

            entity.HasOne(e => e.Script)
                .WithMany(e => e.Metrics);
        });

        modelBuilder.Entity<ScriptVariant>(entity =>
        {
            entity.ToTableWithAnnotations(buildAction: e =>
            {
                e.HasTrigger("TR_script_variants_audit");
            });

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.ScriptId);
            entity.PropertyWithAnnotations(e => e.ScriptInterpreterId);
            entity.PropertyWithAnnotations(e => e.ScriptText);

            entity.HasOne(e => e.Script)
                .WithMany(e => e.Variants);

            entity.HasOne(e => e.ScriptInterpreter);
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
