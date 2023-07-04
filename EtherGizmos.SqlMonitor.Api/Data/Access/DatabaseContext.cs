using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.SqlMonitor.Api.Data.Access;

/// <summary>
/// Provides access to database tables.
/// </summary>
public class DatabaseContext : DbContext
{
    /// <summary>
    /// Provides access to <see cref="Permission"/> records, in 'dbo.permissions'.
    /// </summary>
    public virtual DbSet<Permission> Permissions { get; set; }

    /// <summary>
    /// Provides access to <see cref="Securable"/> records, in 'dbo.securables'.
    /// </summary>
    public virtual DbSet<Securable> Securables { get; set; }

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

        //**********************************************************
        // Add Value Converters

        modelBuilder.AddGlobalValueConverter(new ValueConverter<DateTimeOffset, DateTime>(
            app => app.UtcDateTime,
            db => new DateTimeOffset(db, TimeSpan.Zero)));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<DateTimeOffset?, DateTime?>(
            app => app != null ? app.Value.UtcDateTime : null,
            db => db != null ? new DateTimeOffset((DateTime)db) : null));
    }
}
