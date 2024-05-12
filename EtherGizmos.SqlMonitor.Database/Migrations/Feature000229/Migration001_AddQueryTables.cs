using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;
using FluentMigrator;
using System.Data;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000229;

[CreatedAt(year: 2024, month: 04, day: 28, hour: 11, minute: 00, description: "Create query tables", trackingId: 229)]
public class Migration001_AddQueryTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Create [dbo].[sql_types]
         *  - types of DBMS supported by the application
         */
        Create.Table("sql_types")
            .WithColumn("sql_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        this.AddAuditTriggerV1("sql_types",
            ("sql_type_id", DbType.Int32));

        /*
         * Create [dbo].[queries]
         *  - query definitions being run against servers, without the actual query content
         */
        Create.Table("queries")
            .WithColumn("query_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("run_frequency").AsTime().NotNullable()
            .WithColumn("last_run_at_utc").AsDateTime2().Nullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("is_soft_deleted").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("bucket_column").AsString(100).Nullable()
            .WithColumn("timestamp_utc_column").AsString(100).Nullable()
            .WithColumn("system_id").AsGuid().NotNullable().WithDefault(SystemMethods.NewGuid)
            .WithColumn("securable_id").AsInt32().Nullable();

        this.FixTime("queries", "run_frequency");

        Create.ForeignKey("FK_queries_securable_id")
            .FromTable("queries").ForeignColumn("securable_id")
            .ToTable("securables").PrimaryColumn("securable_id");

        Create.Index("IX_queries_securable_id")
            .OnTable("queries")
            .OnColumn("securable_id");

        this.AddAuditTriggerV1("queries",
            ("query_id", DbType.Guid));

        this.AddSecurableTriggerV1("queries", "securable_id", 390,
            ("query_id", DbType.Guid));

        /*
         * Create [dbo].[query_variants]
         *  - actual query content and a mapping to their DBMS
         */
        Create.Table("query_variants")
            .WithColumn("query_variant_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("query_id").AsGuid().NotNullable()
            .WithColumn("sql_type_id").AsInt32().NotNullable()
            .WithColumn("query_text").AsString(int.MaxValue).NotNullable();

        Create.ForeignKey("FK_query_variants_query_id")
            .FromTable("query_variants").ForeignColumn("query_id")
            .ToTable("queries").PrimaryColumn("query_id");

        Create.Index("IX_query_variants_query_id")
            .OnTable("query_variants")
            .OnColumn("query_id");

        Create.ForeignKey("FK_query_variants_sql_type_id")
            .FromTable("query_variants").ForeignColumn("sql_type_id")
            .ToTable("sql_types").PrimaryColumn("sql_type_id");

        Create.Index("IX_query_variants_sql_type_id")
            .OnTable("query_variants")
            .OnColumn("sql_type_id");

        this.AddAuditTriggerV1("query_variants",
            ("query_variant_id", DbType.Int32));
    }

    public override void Down()
    {
        Delete.Table("query_variants");

        Delete.Table("queries");

        Delete.Table("sql_types");
    }
}
