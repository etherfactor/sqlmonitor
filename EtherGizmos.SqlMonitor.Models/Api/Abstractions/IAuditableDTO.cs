using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EtherGizmos.SqlMonitor.Models.Api.Abstractions;

public interface IAuditableDTO
{
    [Display(Name = "created_at")]
    DateTimeOffset? CreatedAt { get; set; }

    [Display(Name = "created_by_user_id")]
    Guid? CreatedByUserId { get; set; }

    [Display(Name = "modified_at")]
    DateTimeOffset? ModifiedAt { get; set; }

    [Display(Name = "modified_by_user_id")]
    Guid? ModifiedByUserId { get; set; }
}
