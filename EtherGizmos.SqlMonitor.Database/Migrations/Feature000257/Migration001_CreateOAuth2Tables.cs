using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000257;

[CreatedAt(year: 2024, month: 05, day: 14, hour: 21, minute: 00, description: "Create OAuth 2.0 tables", trackingId: 257)]
public class Migration001_CreateOAuth2Tables : MigrationExtension
{
    public override void Up()
    {
        Create.Schema("oauth2");

        Create.Table("application_types")
            .InSchema("oauth2")
            .WithColumn("application_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        Create.Table("applications")
            .InSchema("oauth2")
            .WithColumn("application_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("application_type").AsInt32().Nullable()
            .WithColumn("client_id").AsString(int.MaxValue).Nullable()
            .WithColumn("client_secret").AsString(int.MaxValue).Nullable()
            .WithColumn("client_type").AsString(int.MaxValue).Nullable()
            .WithColumn("concurrency_token").AsString(int.MaxValue).Nullable()
            .WithColumn("consent_type").AsString(int.MaxValue).Nullable()
            .WithColumn("display_name").AsString(int.MaxValue).Nullable()
            .WithColumn("display_names").AsString(int.MaxValue).Nullable()
            .WithColumn("json_web_key_set").AsString(int.MaxValue).Nullable()
            .WithColumn("permissions").AsString(int.MaxValue).Nullable()
            .WithColumn("post_logout_redirect_uris").AsString(int.MaxValue).Nullable()
            .WithColumn("properties").AsString(int.MaxValue).Nullable()
            .WithColumn("redirect_uris").AsString(int.MaxValue).Nullable()
            .WithColumn("requirements").AsString(int.MaxValue).Nullable()
            .WithColumn("settings").AsString(int.MaxValue).Nullable();
    }

    public override void Down()
    {
        Delete.Table("applications")
            .InSchema("oauth2");

        Delete.Table("application_types")
            .InSchema("oauth2");

        Delete.Schema("oauth2");
    }
}
