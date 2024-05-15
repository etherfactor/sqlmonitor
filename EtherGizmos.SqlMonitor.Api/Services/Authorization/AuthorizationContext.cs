using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Authorization.ValueConverters;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.SqlMonitor.Api.Services.Authorization;

[ExcludeFromCodeCoverage]
public class AuthorizationContext : DbContext
{
    public virtual DbSet<OAuth2Application> Applications { get; set; }

    public virtual DbSet<OAuth2Authorization> Authorizations { get; set; }

    public virtual DbSet<OAuth2Scope> Scopes { get; set; }

    public virtual DbSet<OAuth2Token> Tokens { get; set; }

    public AuthorizationContext(DbContextOptions<AuthorizationContext> options, IMigrationManager migrationManager) : base(options)
    {
        migrationManager.EnsureMigrated();
    }

    protected AuthorizationContext(IMigrationManager migrationManager) : base()
    {
        migrationManager.EnsureMigrated();
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //**********************************************************
        // Add Entities

        modelBuilder.Entity<OAuth2Application>(entity =>
        {
            entity.ToTableWithAnnotations("oauth2", buildAction: e =>
            {
                e.HasTrigger("TR_oauth2_applications_audit");
            });

            entity.HasKey(e => e.Id);

            entity.PropertyWithAnnotations(e => e.Id);
            entity.AuditPropertiesWithAnnotations();
            entity.PropertyWithAnnotations(e => e.ApplicationType)
                .HasConversion(new ApplicationTypeValueConverter());
            entity.PropertyWithAnnotations(e => e.ClientId);
            entity.PropertyWithAnnotations(e => e.ClientSecret);
            entity.PropertyWithAnnotations(e => e.ClientType);
            entity.PropertyWithAnnotations(e => e.ConcurrencyToken);
            entity.PropertyWithAnnotations(e => e.ConsentType);
            entity.PropertyWithAnnotations(e => e.DisplayName);
            entity.PropertyWithAnnotations(e => e.DisplayNames);
            entity.PropertyWithAnnotations(e => e.JsonWebKeySet);
            entity.PropertyWithAnnotations(e => e.Permissions);
            entity.PropertyWithAnnotations(e => e.PostLogoutRedirectUris);
            entity.PropertyWithAnnotations(e => e.Properties);
            entity.PropertyWithAnnotations(e => e.RedirectUris);
            entity.PropertyWithAnnotations(e => e.Requirements);
            entity.PropertyWithAnnotations(e => e.Settings);
        });

        modelBuilder.Entity<OAuth2Authorization>();

        modelBuilder.Entity<OAuth2Scope>();

        modelBuilder.Entity<OAuth2Token>();

        //**********************************************************
        // Add Value Converters

        modelBuilder.AddGlobalValueConverter(new ValueConverter<DateTimeOffset, DateTime>(
            app => app.UtcDateTime,
            db => new DateTimeOffset(db, TimeSpan.Zero)));

        modelBuilder.AddGlobalValueConverter(new ValueConverter<DateTimeOffset?, DateTime?>(
            app => app != null ? app.Value.UtcDateTime : null,
            db => db != null ? new DateTimeOffset((DateTime)db, TimeSpan.Zero) : null));
    }
}
