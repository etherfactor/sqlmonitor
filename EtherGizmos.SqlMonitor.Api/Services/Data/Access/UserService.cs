using EtherGizmos.SqlMonitor.Api.Data.Access;
using EtherGizmos.SqlMonitor.Api.Services.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Access;

/// <summary>
/// Provides access to <see cref="User"/> records.
/// </summary>
public class UserService : IUserService
{
    private DatabaseContext Context { get; }

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public UserService(DatabaseContext context)
    {
        Context = context;
    }

    /// <inheritdoc/>
    public void AddOrUpdate(User record)
    {
        if (!Context.Users.Contains(record))
            Context.Users.Add(record);
    }

    /// <inheritdoc/>
    public IQueryable<User> GetQueryable()
    {
        return Context.Users;
    }
}
