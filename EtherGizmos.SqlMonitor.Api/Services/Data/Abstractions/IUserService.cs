using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;

/// <summary>
/// Provides access to <see cref="User"/> records.
/// </summary>
public interface IUserService : IEditableQueryableService<User>
{
}
