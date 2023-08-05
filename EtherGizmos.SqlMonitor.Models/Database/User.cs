using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.Models.Database;

[Table("users")]
public class User : Auditable
{
    [Column("user_id")]
    [SqlDefaultValue]
    public virtual Guid Id { get; set; }

    [Column("username")]
    public virtual string Username { get; set; }

    [Column("password")]
    public virtual string PasswordHash { get; set; }

    [Column("email_address")]
    public virtual string? EmailAddress { get; set; }

    [Column("name")]
    public virtual string? Name { get; set; }

    [Column("is_active")]
    public virtual bool IsActive { get; set; }

    [Column("is_soft_deleted")]
    public virtual bool IsSoftDeleted { get; set; }

    [Column("is_administrator")]
    public virtual bool IsAdministrator { get; set; }

    [Column("last_login_at_utc")]
    public virtual DateTimeOffset? LastLoginAtUtc { get; set; }

    [Column("principal_id")]
    public virtual Guid PrincipalId { get; set; }

    public virtual Principal Principal { get; set; } = new Principal() { Type = PrincipalType.User };

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public User()
    {
        Username = null!;
        PasswordHash = null!;
    }

    public Task EnsureValid(IQueryable<User> records)
    {
        return Task.CompletedTask;
    }
}
