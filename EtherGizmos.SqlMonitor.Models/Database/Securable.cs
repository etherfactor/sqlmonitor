using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtherGizmos.SqlMonitor.Models.Database;

public class Securable : Auditable
{
    public virtual string Id { get; set; }

    public virtual string Name { get; set; }

    public virtual string? Description { get; set; }

    /// <summary>
    /// Not intended for direct use.
    /// </summary>
    public Securable()
    {
        Id = null!;
        Name = null!;
    }
}
