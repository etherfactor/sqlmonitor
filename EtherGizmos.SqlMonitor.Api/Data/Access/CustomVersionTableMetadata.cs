using FluentMigrator.Runner.VersionTableInfo;

namespace EtherGizmos.SqlMonitor.Api.Data.Access;

public class CustomVersionTableMetadata : IVersionTableMetaData
{
    public object? ApplicationContext { get; set; }

    public bool OwnsSchema => true;

    public string SchemaName => "dbo";

    public string TableName => "migration_history";

    public string ColumnName => "version";

    public string DescriptionColumnName => "description";

    public string UniqueIndexName => "UX_migration_history_version";

    public string AppliedOnColumnName => "applied_at_utc";
}
