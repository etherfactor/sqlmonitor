using EtherGizmos.SqlMonitor.Shared.Models.Database;

namespace EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;

/// <summary>
/// Provides access to <see cref="Script"/> records.
/// </summary>
public interface IScriptService : IEditableQueryableService<Script>
{
}
