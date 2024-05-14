using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

[ExcludeFromCodeCoverage]
public class AuthorizationContext : DbContext
{
    public AuthorizationContext(DbContextOptions options, IMigrationManager migrationManager) : base(options)
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

        //**********************************************************
        // Add Value Converters
    }
}
