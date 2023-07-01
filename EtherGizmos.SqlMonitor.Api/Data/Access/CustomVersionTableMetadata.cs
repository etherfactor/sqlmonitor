using FluentMigrator.Runner.VersionTableInfo;

namespace EtherGizmos.SqlMonitor.Api.Data.Access;

/// <summary>
/// Overrides default FluentMigrator version table naming.
/// </summary>
public class CustomVersionTableMetadata : IVersionTableMetaData
{
    /// <inheritdoc/>
    public object? ApplicationContext { get; set; }

    /// <inheritdoc/>
    public bool OwnsSchema => true;

    /// <inheritdoc/>
    public string SchemaName => "dbo";

    /// <inheritdoc/>
    public string TableName => "migration_history";

    /// <inheritdoc/>
    public string ColumnName => "version";

    /// <inheritdoc/>
    public string DescriptionColumnName => "description";

    /// <inheritdoc/>
    public string UniqueIndexName => "UX_migration_history_version";

    /// <inheritdoc/>
    public string AppliedOnColumnName => "applied_at_utc";
}
