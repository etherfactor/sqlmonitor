using EtherGizmos.SqlMonitor.Database.Core;
using EtherGizmos.SqlMonitor.Database.Extensions;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000266;

[CreatedAt(year: 2024, month: 05, day: 14, hour: 21, minute: 00, description: "Create OAuth 2.0 tables", trackingId: 266)]
public class Migration001_CreateOAuth2Tables : MigrationExtension
{
    public override void Up()
    {
        Create.Table("oauth2_application_types")
            .WithColumn("oauth2_application_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        Create.Table("oauth2_authorization_types")
            .WithColumn("oauth2_authorization_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        Create.Table("oauth2_client_types")
            .WithColumn("oauth2_client_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        Create.Table("oauth2_consent_types")
            .WithColumn("oauth2_consent_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        Create.Table("oauth2_status_types")
            .WithColumn("oauth2_status_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        Create.Table("oauth2_token_types")
            .WithColumn("oauth2_token_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        Create.Table("oauth2_applications")
            .WithColumn("oauth2_application_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("oauth2_application_type_id").AsInt32().Nullable()
            .WithColumn("client_id").AsGuid().Nullable()
            .WithColumn("client_secret").AsString(int.MaxValue).Nullable()
            .WithColumn("oauth2_client_type_id").AsInt32().Nullable()
            .WithColumn("concurrency_token").AsString(int.MaxValue).Nullable()
            .WithColumn("oauth2_consent_type_id").AsInt32().Nullable()
            .WithColumn("display_name").AsString(int.MaxValue).Nullable()
            .WithColumn("display_names").AsString(int.MaxValue).Nullable()
            .WithColumn("json_web_key_set").AsString(int.MaxValue).Nullable()
            .WithColumn("permissions").AsString(int.MaxValue).Nullable()
            .WithColumn("post_logout_redirect_uris").AsString(int.MaxValue).Nullable()
            .WithColumn("properties").AsString(int.MaxValue).Nullable()
            .WithColumn("redirect_uris").AsString(int.MaxValue).Nullable()
            .WithColumn("requirements").AsString(int.MaxValue).Nullable()
            .WithColumn("settings").AsString(int.MaxValue).Nullable();

        Create.ForeignKey("FK_oauth2_applications_application_type_id")
            .FromTable("oauth2_applications").ForeignColumn("oauth2_application_type_id")
            .ToTable("oauth2_application_types").PrimaryColumn("oauth2_application_type_id");

        Create.ForeignKey("FK_oauth2_applications_client_type_id")
            .FromTable("oauth2_applications").ForeignColumn("oauth2_client_type_id")
            .ToTable("oauth2_client_types").PrimaryColumn("oauth2_client_type_id");

        Create.ForeignKey("FK_oauth2_applications_consent_type_id")
            .FromTable("oauth2_applications").ForeignColumn("oauth2_consent_type_id")
            .ToTable("oauth2_consent_types").PrimaryColumn("oauth2_consent_type_id");

        Create.Table("oauth2_authorizations")
            .WithColumn("oauth2_authorization_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("oauth2_application_id").AsInt32().Nullable()
            .WithColumn("concurrency_token").AsString(int.MaxValue).Nullable()
            .WithColumn("properties").AsString(int.MaxValue).Nullable()
            .WithColumn("scopes").AsString(int.MaxValue).Nullable()
            .WithColumn("oauth2_status_type_id").AsInt32().Nullable()
            .WithColumn("subject").AsString(int.MaxValue).Nullable()
            .WithColumn("oauth2_authorization_type_id").AsInt32().Nullable();

        Create.ForeignKey("FK_oauth2_authorizations_application_id")
            .FromTable("oauth2_authorizations").ForeignColumn("oauth2_application_id")
            .ToTable("oauth2_applications").PrimaryColumn("oauth2_application_id");

        Create.ForeignKey("FK_oauth2_authorizations_status_type_id")
            .FromTable("oauth2_authorizations").ForeignColumn("oauth2_status_type_id")
            .ToTable("oauth2_status_types").PrimaryColumn("oauth2_status_type_id");

        Create.ForeignKey("FK_oauth2_authorizations_authorization_type_id")
            .FromTable("oauth2_authorizations").ForeignColumn("oauth2_authorization_type_id")
            .ToTable("oauth2_authorization_types").PrimaryColumn("oauth2_authorization_type_id");

        Create.Table("oauth2_scopes")
            .WithColumn("oauth2_scope_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("concurrency_token").AsString(int.MaxValue).Nullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("descriptions").AsString(int.MaxValue).Nullable()
            .WithColumn("display_name").AsString(int.MaxValue).Nullable()
            .WithColumn("display_names").AsString(int.MaxValue).Nullable()
            .WithColumn("name").AsString(int.MaxValue).Nullable()
            .WithColumn("properties").AsString(int.MaxValue).Nullable()
            .WithColumn("resources").AsString(int.MaxValue).Nullable();

        Create.Table("oauth2_tokens")
            .WithColumn("oauth2_token_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("oauth2_application_id").AsInt32().Nullable()
            .WithColumn("oauth2_authorization_id").AsInt32().Nullable()
            .WithColumn("concurrency_token").AsString(int.MaxValue).Nullable()
            .WithColumn("expires_at_utc").AsDateTime2().Nullable()
            .WithColumn("payload").AsString(int.MaxValue).Nullable()
            .WithColumn("properties").AsString(int.MaxValue).Nullable()
            .WithColumn("redeemed_at_utc").AsDateTime2().Nullable()
            .WithColumn("reference_id").AsString(int.MaxValue).Nullable()
            .WithColumn("oauth2_status_type_id").AsInt32().Nullable()
            .WithColumn("subject").AsString(int.MaxValue).Nullable()
            .WithColumn("oauth2_token_type_id").AsInt32().Nullable();

        Create.ForeignKey("FK_oauth2_tokens_application_id")
            .FromTable("oauth2_tokens").ForeignColumn("oauth2_application_id")
            .ToTable("oauth2_applications").PrimaryColumn("oauth2_application_id");

        Create.ForeignKey("FK_oauth2_tokens_authorization_id")
            .FromTable("oauth2_tokens").ForeignColumn("oauth2_authorization_id")
            .ToTable("oauth2_authorizations").PrimaryColumn("oauth2_authorization_id");

        Create.ForeignKey("FK_oauth2_tokens_status_type_id")
            .FromTable("oauth2_tokens").ForeignColumn("oauth2_status_type_id")
            .ToTable("oauth2_status_types").PrimaryColumn("oauth2_status_type_id");

        Create.ForeignKey("FK_oauth2_tokens_token_type_id")
            .FromTable("oauth2_tokens").ForeignColumn("oauth2_token_type_id")
            .ToTable("oauth2_token_types").PrimaryColumn("oauth2_token_type_id");
    }

    public override void Down()
    {
        Delete.Table("oauth2_tokens");

        Delete.Table("oauth2_scopes");

        Delete.Table("oauth2_authorizations");

        Delete.Table("oauth2_applications");

        Delete.Table("oauth2_token_types");

        Delete.Table("oauth2_status_types");

        Delete.Table("oauth2_consent_types");

        Delete.Table("oauth2_client_types");

        Delete.Table("oauth2_authorization_types");

        Delete.Table("oauth2_application_types");
    }
}
