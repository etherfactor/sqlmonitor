using OpenIddict.EntityFrameworkCore.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Api.Services.Authorization;

[Table("authorizations", Schema = "oauth2")]
public class OAuth2Authorization : OpenIddictEntityFrameworkCoreAuthorization<int, OAuth2Application, OAuth2Token>
{
    [Column("created_at_utc")]
    public override DateTime? CreationDate { get => base.CreationDate; set => base.CreationDate = value; }

    [Column("created_by_user_id")]
    public Guid? CreatedByUserId { get; set; }

    [Column("modified_at_utc")]
    public DateTimeOffset? ModifiedAt { get; set; }

    [Column("modified_by_user_id")]
    public Guid? ModifiedByUserId { get; set; }

    [Column("application_id")]
    public virtual int? ApplicationId { get; set; }

    [Column("concurrency_token")]
    public override string? ConcurrencyToken { get => base.ConcurrencyToken; set => base.ConcurrencyToken = value; }

    [Column("authorization_id")]
    public override int Id { get => base.Id; set => base.Id = value; }

    [Column("properties")]
    public override string? Properties { get => base.Properties; set => base.Properties = value; }

    [Column("scopes")]
    public override string? Scopes { get => base.Scopes; set => base.Scopes = value; }

    [Column("status_type_id")]
    public override string? Status { get => base.Status; set => base.Status = value; }

    [Column("subject")]
    public override string? Subject { get => base.Subject; set => base.Subject = value; }

    [Column("authorization_type_id")]
    public override string? Type { get => base.Type; set => base.Type = value; }
}
