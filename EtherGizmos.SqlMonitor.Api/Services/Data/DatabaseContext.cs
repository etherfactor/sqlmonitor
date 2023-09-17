using EtherGizmos.SqlMonitor.Api.Extensions;
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
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseContext"/> class. The
    /// <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/> method will be called to configure the database (and
    /// other options) to be used for this context.
    /// </summary>
    protected DatabaseContext() : base()
    {
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

            entity.HasMany(e => e.QueryBlacklists)
                .WithOne(e => e.Instance)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(e => e.InstanceId);

            entity.HasMany(e => e.QueryDatabaseOverrides)
                .WithOne(e => e.Instance)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(e => e.InstanceId);

            entity.HasMany(e => e.QueryWhitelists)
                .WithOne(e => e.Instance)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(e => e.InstanceId);
        });

        modelBuilder.Entity<InstanceQueryBlacklist>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => new { e.InstanceId, e.QueryId });

            entity.PropertyWithAnnotations(e => e.InstanceId);
            entity.PropertyWithAnnotations(e => e.QueryId);
        });

        modelBuilder.Entity<InstanceQueryDatabase>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => new { e.InstanceId, e.QueryId });

            entity.PropertyWithAnnotations(e => e.InstanceId);
            entity.PropertyWithAnnotations(e => e.QueryId);
            entity.PropertyWithAnnotations(e => e.DatabaseOverride);
        });

        modelBuilder.Entity<InstanceQueryWhitelist>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => new { e.InstanceId, e.QueryId });

            entity.PropertyWithAnnotations(e => e.InstanceId);
            entity.PropertyWithAnnotations(e => e.QueryId);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);

            entity.HasMany(e => e.Securables)
                .WithMany(e => e.Permissions)
                .UsingEntity<SecurablePermission>();
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
        });

        modelBuilder.Entity<Securable>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.Name);
            entity.PropertyWithAnnotations(e => e.Description);

            entity.HasMany(e => e.Permissions)
                .WithMany(e => e.Securables)
                .UsingEntity<SecurablePermission>();
        });

        modelBuilder.Entity<SecurablePermission>(entity =>
        {
            entity.ToTableWithAnnotations();

            entity.HasKey(e => new { e.SecurableId, e.PermissionId });

            entity.PropertyWithAnnotations(e => e.SecurableId);
            entity.PropertyWithAnnotations(e => e.PermissionId);
            entity.AuditPropertiesWithAnnotations();
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

        modelBuilder.AddGlobalValueConverter(new ValueConverter<PrincipalType, string>(
            app => PrincipalTypeConverter.ToString(app),
            db => PrincipalTypeConverter.FromString(db)));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<PrincipalType?, string?>(
            app => PrincipalTypeConverter.ToStringOrDefault(app),
            db => PrincipalTypeConverter.FromStringOrDefault(db)));
    }
}
