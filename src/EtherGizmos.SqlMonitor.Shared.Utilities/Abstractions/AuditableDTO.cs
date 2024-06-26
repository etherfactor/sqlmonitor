﻿namespace EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;

public abstract class AuditableDTO : IAuditableDTO
{
    public DateTimeOffset? CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

    public Guid? ModifiedByUserId { get; set; }
}
