using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;

/// <summary>
/// Provides access to <see cref="Script"/> records.
/// </summary>
public interface IScriptService : IEditableQueryableService<Script>
{
}
