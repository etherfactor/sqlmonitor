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
    /// Provides access to <see cref="Instance"/> records, in 'dbo.instances'.
    /// </summary>
    public virtual DbSet<Instance> Instances { get; set; }

    /// <summary>
    /// Provides access to <see cref="InstanceMetricBySecond"/> records, in 'dbo.instance_metrics_by_second'.
    /// </summary>
    public virtual DbSet<InstanceMetricBySecond> InstanceMetricsBySecond { get; set; }

    /// <summary>
    /// Provides access to <see cref="MetricBucket"/> records, in 'dbo.metric_buckets'.
    /// </summary>
    public virtual DbSet<MetricBucket> MetricBuckets { get; set; }

    /// <summary>
    /// Provides access to <see cref="Metric"/> records, in 'dbo.metrics'.
    /// </summary>
    public virtual DbSet<Metric> Metrics { get; set; }

    /// <summary>
    /// Provides access to <see cref="MonitoredSystem"/> records, in 'dbo.monitored
    /// </summary>
    public virtual DbSet<MonitoredSystem> MonitoredSystems { get; set; }

    /// <summary>
    /// Provides access to <see cref="Permission"/> records, in 'dbo.permissions'.
    /// </summary>
    public virtual DbSet<Permission> Permissions { get; set; }

    /// <summary>
    /// Provides access to <see cref="Query"/> records, in 'dbo.queries'.
    /// </summary>
    public virtual DbSet<Query> Queries { get; set; }

    /// <summary>
    /// Provides access to <see cref="Securable"/> records, in 'dbo.securables'.
    /// </summary>
    public virtual DbSet<Securable> Securables { get; set; }

    /// <summary>
    /// Provides access to <see cref="User"/> records, in 'dbo.users'.
    /// </summary>
    public virtual DbSet<User> Users { get; set; }

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

        modelBuilder.Entity<Instance>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id)
                .HasDefaultValueSql();
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);
            entity.PropertyWithAnnotations(e => e.Address);
            entity.PropertyWithAnnotations(e => e.Port);

            entity.HasMany(e => e.MetricsByDay)
                .WithOne(e => e.Instance)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(e => e.InstanceId);

            entity.HasMany(e => e.MetricsByHour)
                .WithOne(e => e.Instance)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(e => e.InstanceId);

            entity.HasMany(e => e.MetricsByMinute)
                .WithOne(e => e.Instance)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(e => e.InstanceId);

            entity.HasMany(e => e.MetricsBySecond)
                .WithOne(e => e.Instance)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(e => e.InstanceId);
        });

        modelBuilder.Entity<InstanceMetricByDay>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => new { e.InstanceId, e.MeasuredAtUtc, e.MetricId, e.MetricBucketId });

            entity.PropertyWithAnnotations(e => e.InstanceId);
            entity.PropertyWithAnnotations(e => e.MeasuredAtUtc);
            entity.PropertyWithAnnotations(e => e.MetricId);
            entity.PropertyWithAnnotations(e => e.MetricBucketId);
            entity.PropertyWithAnnotations(e => e.Value);
            entity.PropertyWithAnnotations(e => e.SeverityType);
        });

        modelBuilder.Entity<InstanceMetricByHour>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => new { e.InstanceId, e.MeasuredAtUtc, e.MetricId, e.MetricBucketId });

            entity.PropertyWithAnnotations(e => e.InstanceId);
            entity.PropertyWithAnnotations(e => e.MeasuredAtUtc);
            entity.PropertyWithAnnotations(e => e.MetricId);
            entity.PropertyWithAnnotations(e => e.MetricBucketId);
            entity.PropertyWithAnnotations(e => e.Value);
            entity.PropertyWithAnnotations(e => e.SeverityType);
        });

        modelBuilder.Entity<InstanceMetricByMinute>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => new { e.InstanceId, e.MeasuredAtUtc, e.MetricId, e.MetricBucketId });

            entity.PropertyWithAnnotations(e => e.InstanceId);
            entity.PropertyWithAnnotations(e => e.MeasuredAtUtc);
            entity.PropertyWithAnnotations(e => e.MetricId);
            entity.PropertyWithAnnotations(e => e.MetricBucketId);
            entity.PropertyWithAnnotations(e => e.Value);
            entity.PropertyWithAnnotations(e => e.SeverityType);
        });

        modelBuilder.Entity<InstanceMetricBySecond>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => new { e.InstanceId, e.MeasuredAtUtc, e.MetricId, e.MetricBucketId });

            entity.PropertyWithAnnotations(e => e.InstanceId);
            entity.PropertyWithAnnotations(e => e.MeasuredAtUtc);
            entity.PropertyWithAnnotations(e => e.MetricId);
            entity.PropertyWithAnnotations(e => e.MetricBucketId);
            entity.PropertyWithAnnotations(e => e.Value);
            entity.PropertyWithAnnotations(e => e.SeverityType);
        });

        modelBuilder.Entity<Metric>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id)
                .HasDefaultValueSql();
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);
            entity.PropertyWithAnnotations(e => e.AggregateType);

            entity.HasMany(e => e.Severities)
                .WithOne(e => e.Metric)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(e => e.MetricId);
        });

        modelBuilder.Entity<MetricBucket>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id)
                .HasDefaultValueSql();
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
        });

        modelBuilder.Entity<MetricSeverity>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => new { e.MetricId, e.SeverityType });

            entity.PropertyWithAnnotations(e => e.MetricId);
            entity.PropertyWithAnnotations(e => e.SeverityType);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.MinimumValue);
            entity.PropertyWithAnnotations(e => e.MaximumValue);
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

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);
        });

        modelBuilder.Entity<Principal>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id)
                .HasDefaultValueSql();
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Type);
        });

        modelBuilder.Entity<Query>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id)
                .HasDefaultValueSql();
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.SystemId)
                .HasDefaultValueSql();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);
            entity.PropertyWithAnnotations(e => e.IsActive);
            entity.PropertyWithAnnotations(e => e.IsSoftDeleted);
            entity.PropertyWithAnnotations(e => e.SqlText);
            entity.PropertyWithAnnotations(e => e.RunFrequency);
            entity.PropertyWithAnnotations(e => e.LastRunAtUtc);
            entity.PropertyWithAnnotations(e => e.TimestampUtcExpression);
            entity.PropertyWithAnnotations(e => e.BucketExpression);

            entity.Ignore(e => e.NextRunAtUtc);

            entity.HasMany(e => e.InstanceBlacklists)
                .WithOne(e => e.Query)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(e => e.QueryId);

            entity.HasMany(e => e.InstanceDatabaseOverrides)
                .WithOne(e => e.Query)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(e => e.QueryId);

            entity.HasMany(e => e.InstanceWhitelists)
                .WithOne(e => e.Query)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(e => e.QueryId);

            entity.HasMany(e => e.Metrics)
                .WithOne(e => e.Query)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(e => e.QueryId);
        });

        modelBuilder.Entity<QueryInstanceBlacklist>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => new { e.InstanceId, e.QueryId });

            entity.PropertyWithAnnotations(e => e.InstanceId);
            entity.PropertyWithAnnotations(e => e.QueryId);
        });

        modelBuilder.Entity<QueryInstanceDatabase>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => new { e.InstanceId, e.QueryId });

            entity.PropertyWithAnnotations(e => e.InstanceId);
            entity.PropertyWithAnnotations(e => e.QueryId);
            entity.PropertyWithAnnotations(e => e.DatabaseOverride);
        });

        modelBuilder.Entity<QueryInstanceWhitelist>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => new { e.InstanceId, e.QueryId });

            entity.PropertyWithAnnotations(e => e.InstanceId);
            entity.PropertyWithAnnotations(e => e.QueryId);
        });

        modelBuilder.Entity<QueryMetric>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => new { e.QueryId, e.MetricId });

            entity.PropertyWithAnnotations(e => e.QueryId);
            entity.PropertyWithAnnotations(e => e.MetricId);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.ValueExpression);

            entity.HasMany(e => e.Severities)
                .WithOne(e => e.QueryMetric)
                .HasPrincipalKey(e => new { e.QueryId, e.MetricId })
                .HasForeignKey(e => new { e.QueryId, e.MetricId });
        });

        modelBuilder.Entity<QueryMetricSeverity>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => new { e.QueryId, e.MetricId, e.SeverityType });

            entity.PropertyWithAnnotations(e => e.QueryId);
            entity.PropertyWithAnnotations(e => e.MetricId);
            entity.PropertyWithAnnotations(e => e.SeverityType);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.MinimumExpression);
            entity.PropertyWithAnnotations(e => e.MaximumExpression);
        });

        modelBuilder.Entity<Securable>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.PropertyWithAnnotations(e => e.Type);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id)
                .HasDefaultValueSql();
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Username);
            entity.PropertyWithAnnotations(e => e.PasswordHash);
            entity.PropertyWithAnnotations(e => e.EmailAddress);
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.IsActive);
            entity.PropertyWithAnnotations(e => e.IsSoftDeleted);
            entity.PropertyWithAnnotations(e => e.IsAdministrator);
            entity.PropertyWithAnnotations(e => e.LastLoginAtUtc);
            entity.PropertyWithAnnotations(e => e.PrincipalId);

            entity.HasOne(e => e.Principal);
        });

        //**********************************************************
        // Add Value Converters

        modelBuilder.AddGlobalValueConverter(new ValueConverter<DateTimeOffset, DateTime>(
            app => app.UtcDateTime,
            db => new DateTimeOffset(db, TimeSpan.Zero)));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<DateTimeOffset?, DateTime?>(
            app => app != null ? app.Value.UtcDateTime : null,
            db => db != null ? new DateTimeOffset((DateTime)db, TimeSpan.Zero) : null));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<AggregateType, string>(
            app => AggregateTypeConverter.ToString(app),
            db => AggregateTypeConverter.FromString(db)));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<AggregateType?, string?>(
            app => AggregateTypeConverter.ToStringOrDefault(app),
            db => AggregateTypeConverter.FromStringOrDefault(db)));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<PrincipalType, string>(
            app => PrincipalTypeConverter.ToString(app),
            db => PrincipalTypeConverter.FromString(db)));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<PrincipalType?, string?>(
            app => PrincipalTypeConverter.ToStringOrDefault(app),
            db => PrincipalTypeConverter.FromStringOrDefault(db)));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<SeverityType, string>(
            app => SeverityTypeConverter.ToString(app),
            db => SeverityTypeConverter.FromString(db)));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<SeverityType?, string?>(
            app => SeverityTypeConverter.ToStringOrDefault(app),
            db => SeverityTypeConverter.FromStringOrDefault(db)));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<SecurableType, int>(
            app => SecurableTypeConverter.ToInteger(app),
            db => SecurableTypeConverter.FromInteger(db)));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<SecurableType?, int?>(
            app => SecurableTypeConverter.ToIntegerOrDefault(app),
            db => SecurableTypeConverter.FromIntegerOrDefault(db)));
    }
}
