using EtherGizmos.SqlMonitor.Database.Core;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000266;

[CreatedAt(year: 2024, month: 05, day: 15, hour: 22, minute: 00, description: "Load OAuth 2.0 tables", trackingId: 266)]
public class Migration002_LoadOAuth2Tables : MigrationExtension
{
    public override void Up()
    {
        Merge.IntoTable("oauth2_application_types")
            .Row(new { oauth2_application_type_id = 10, name = "Native", description = null as string })
            .Row(new { oauth2_application_type_id = 20, name = "Web", description = null as string })
            .Row(new { oauth2_application_type_id = 100, name = "Agent", description = null as string })
            .Match(e => e.oauth2_application_type_id);

        Merge.IntoTable("oauth2_authorization_types")
            .Row(new { oauth2_authorization_type_id = 10, name = "Permanent", description = null as string })
            .Row(new { oauth2_authorization_type_id = 20, name = "Ad-hoc", description = null as string })
            .Match(e => e.oauth2_authorization_type_id);

        Merge.IntoTable("oauth2_client_types")
            .Row(new { oauth2_client_type_id = 10, name = "Public", description = null as string })
            .Row(new { oauth2_client_type_id = 20, name = "Confidential", description = null as string })
            .Match(e => e.oauth2_client_type_id);

        Merge.IntoTable("oauth2_consent_types")
            .Row(new { oauth2_consent_type_id = 10, name = "Explicit", description = null as string })
            .Row(new { oauth2_consent_type_id = 20, name = "External", description = null as string })
            .Row(new { oauth2_consent_type_id = 30, name = "Implicit", description = null as string })
            .Row(new { oauth2_consent_type_id = 40, name = "Systematic", description = null as string })
            .Match(e => e.oauth2_consent_type_id);

        Merge.IntoTable("oauth2_status_types")
            .Row(new { oauth2_status_type_id = 10, name = "Valid", description = null as string })
            .Row(new { oauth2_status_type_id = 20, name = "Inactive", description = null as string })
            .Row(new { oauth2_status_type_id = 30, name = "Redeemed", description = null as string })
            .Row(new { oauth2_status_type_id = 40, name = "Rejected", description = null as string })
            .Row(new { oauth2_status_type_id = 50, name = "Revoked", description = null as string })
            .Match(e => e.oauth2_status_type_id);

        Merge.IntoTable("oauth2_token_types")
            .Row(new { oauth2_token_type_id = 10, name = "Bearer", description = null as string })
            .Row(new { oauth2_token_type_id = 20, name = "Access Token", description = null as string })
            .Row(new { oauth2_token_type_id = 30, name = "Refresh Token", description = null as string })
            .Match(e => e.oauth2_token_type_id);
    }

    public override void Down()
    {
    }
}
