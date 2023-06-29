using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtherGizmos.SqlMonitor.Models.Database.Abstractions;

public interface IAuditable
{
    [Column("created_at")]
    DateTimeOffset CreatedAt { get; set; }

    [Column("created_by_user_id")]
    Guid CreatedByUserId { get; set; }

    [Column("modified_at")]
    DateTimeOffset ModifiedAt { get; set; }

    [Column("modified_by_user_id")]
    Guid ModifiedByUserId { get; set; }
}
