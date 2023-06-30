namespace EtherGizmos.SqlMonitor.Models.Api.Abstractions;

public class AuditableDTO : IAuditableDTO
{
    public DateTimeOffset? CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

    public Guid? ModifiedByUserId { get; set; }
}
