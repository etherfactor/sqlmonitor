using EtherGizmos.SqlMonitor.Api.Data.Access;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services;

public class SecurableService : ISecurableService
{
    private DatabaseContext Context { get; }

    public SecurableService(DatabaseContext context)
    {
        Context = context;
    }

    public async Task AddOrUpdate(Securable record)
    {
        if (!Context.Securables.Contains(record))
            Context.Securables.Add(record);

        await Context.SaveChangesAsync();
    }

    public IQueryable<Securable> GetQueryable()
    {
        return Context.Securables;
    }
}
