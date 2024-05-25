using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.Abstractions;

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
