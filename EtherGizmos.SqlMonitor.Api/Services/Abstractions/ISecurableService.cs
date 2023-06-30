using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Abstractions;

public interface ISecurableService
{
    IQueryable<Securable> GetQueryable();

    Task AddOrUpdate(Securable securable);
}
