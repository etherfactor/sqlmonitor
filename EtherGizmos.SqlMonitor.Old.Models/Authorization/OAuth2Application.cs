using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Authorization;

[Table("oauth2_applications")]
public class OAuth2Application : OpenIddictEntityFrameworkCoreApplication<int, OAuth2Authorization, OAuth2Token>, IAuditable
{
    [Column("created_at_utc")]
    public DateTimeOffset? CreatedAt { get; set; }

    [Column("created_by_user_id")]
    public Guid? CreatedByUserId { get; set; }

    [Column("modified_at_utc")]
    public DateTimeOffset? ModifiedAt { get; set; }

    [Column("modified_by_user_id")]
    public Guid? ModifiedByUserId { get; set; }

    [Column("oauth2_application_type_id")]
    public override string? ApplicationType { get => base.ApplicationType; set => base.ApplicationType = value; }

    [Column("client_id")]
    public override string? ClientId { get => base.ClientId; set => base.ClientId = value; }

    [Column("client_secret")]
    public override string? ClientSecret { get => base.ClientSecret; set => base.ClientSecret = value; }

    [Column("oauth2_client_type_id")]
    public override string? ClientType { get => base.ClientType; set => base.ClientType = value; }

    [Column("concurrency_token")]
    public override string? ConcurrencyToken { get => base.ConcurrencyToken; set => base.ConcurrencyToken = value; }

    [Column("oauth2_consent_type_id")]
    public override string? ConsentType { get => base.ConsentType; set => base.ConsentType = value; }

    [Column("display_name")]
    public override string? DisplayName { get => base.DisplayName; set => base.DisplayName = value; }

    [Column("display_names")]
    public override string? DisplayNames { get => base.DisplayNames; set => base.DisplayNames = value; }

    [Column("oauth2_application_id")]
    public override int Id { get => base.Id; set => base.Id = value; }

    [Column("json_web_key_set")]
    public override string? JsonWebKeySet { get => base.JsonWebKeySet; set => base.JsonWebKeySet = value; }

    [Column("permissions")]
    public override string? Permissions { get => base.Permissions; set => base.Permissions = value; }

    [Column("post_logout_redirect_uris")]
    public override string? PostLogoutRedirectUris { get => base.PostLogoutRedirectUris; set => base.PostLogoutRedirectUris = value; }

    [Column("properties")]
    public override string? Properties { get => base.Properties; set => base.Properties = value; }

    [Column("redirect_uris")]
    public override string? RedirectUris { get => base.RedirectUris; set => base.RedirectUris = value; }

    [Column("requirements")]
    public override string? Requirements { get => base.Requirements; set => base.Requirements = value; }

    [Column("settings")]
    public override string? Settings { get => base.Settings; set => base.Settings = value; }
}
