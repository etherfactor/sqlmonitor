using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtherGizmos.SqlMonitor.Models.Database.Abstractions;

public class Auditable : IAuditable
{
    public virtual DateTimeOffset CreatedAt { get; set; }

    public virtual Guid CreatedByUserId { get; set; }

    public virtual DateTimeOffset ModifiedAt { get; set; }

    public virtual Guid ModifiedByUserId { get; set; }
}
