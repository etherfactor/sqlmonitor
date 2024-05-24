using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Authorization;

[Table("oauth2_scopes")]
public class OAuth2Scope : OpenIddictEntityFrameworkCoreScope<int>, IAuditable
{
    [Column("created_at_utc")]
    public DateTimeOffset? CreatedAt { get; set; }

    [Column("created_by_user_id")]
    public Guid? CreatedByUserId { get; set; }

    [Column("modified_at_utc")]
    public DateTimeOffset? ModifiedAt { get; set; }

    [Column("modified_by_user_id")]
    public Guid? ModifiedByUserId { get; set; }

    [Column("concurrency_token")]
    public override string? ConcurrencyToken { get => base.ConcurrencyToken; set => base.ConcurrencyToken = value; }

    [Column("description")]
    public override string? Description { get => base.Description; set => base.Description = value; }

    [Column("descriptions")]
    public override string? Descriptions { get => base.Descriptions; set => base.Descriptions = value; }

    [Column("display_name")]
    public override string? DisplayName { get => base.DisplayName; set => base.DisplayName = value; }

    [Column("display_names")]
    public override string? DisplayNames { get => base.DisplayNames; set => base.DisplayNames = value; }

    [Column("oauth2_scope_id")]
    public override int Id { get => base.Id; set => base.Id = value; }

    [Column("name")]
    public override string? Name { get => base.Name; set => base.Name = value; }

    [Column("properties")]
    public override string? Properties { get => base.Properties; set => base.Properties = value; }

    [Column("resources")]
    public override string? Resources { get => base.Resources; set => base.Resources = value; }
}
