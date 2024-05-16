using EtherGizmos.SqlMonitor.Database.Core;

namespace EtherGizmos.SqlMonitor.Database.Migrations.Feature000257;

[CreatedAt(year: 2024, month: 05, day: 15, hour: 22, minute: 00, description: "Load OAuth 2.0 tables", trackingId: 257)]
public class Migration002_LoadOAuth2Tables : MigrationExtension
{
    public override void Up()
    {
        Merge.IntoTable("application_types")
            .InSchema("oauth2")
            .Row(new { application_type_id = 10, name = "Native", description = null as string })
            .Row(new { application_type_id = 20, name = "Web", description = null as string })
            .Match(e => e.application_type_id);

        Merge.IntoTable("authorization_types")
            .InSchema("oauth2")
            .Row(new { authorization_type_id = 10, name = "Permanent", description = null as string })
            .Row(new { authorization_type_id = 20, name = "Ad-hoc", description = null as string })
            .Match(e => e.authorization_type_id);

        Merge.IntoTable("client_types")
            .InSchema("oauth2")
            .Row(new { client_type_id = 10, name = "Public", description = null as string })
            .Row(new { client_type_id = 20, name = "Confidential", description = null as string })
            .Match(e => e.client_type_id);

        Merge.IntoTable("consent_types")
            .InSchema("oauth2")
            .Row(new { consent_type_id = 10, name = "Explicit", description = null as string })
            .Row(new { consent_type_id = 20, name = "External", description = null as string })
            .Row(new { consent_type_id = 30, name = "Implicit", description = null as string })
            .Row(new { consent_type_id = 40, name = "Systematic", description = null as string })
            .Match(e => e.consent_type_id);

        Merge.IntoTable("status_types")
            .InSchema("oauth2")
            .Row(new { status_type_id = 10, name = "Valid", description = null as string })
            .Row(new { status_type_id = 20, name = "Inactive", description = null as string })
            .Row(new { status_type_id = 30, name = "Redeemed", description = null as string })
            .Row(new { status_type_id = 40, name = "Rejected", description = null as string })
            .Row(new { status_type_id = 50, name = "Revoked", description = null as string })
            .Match(e => e.status_type_id);

        Merge.IntoTable("token_types")
            .InSchema("oauth2")
            .Row(new { token_type_id = 10, name = "Bearer", description = null as string })
            .Match(e => e.token_type_id);
    }

    public override void Down()
    {
    }
}
