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

    public async Task AddOrUpdate(Securable securable)
    {
        if (!Context.Securables.Contains(securable))
            Context.Securables.Add(securable);

        await Context.SaveChangesAsync();
    }

    public IQueryable<Securable> GetQueryable()
    {
        return Context.Securables;
    }
}
