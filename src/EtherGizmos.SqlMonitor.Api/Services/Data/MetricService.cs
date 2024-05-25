using EtherGizmos.SqlMonitor.Api.Services.Authorization;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides access to <see cref="Metric"/> records.
/// </summary>
public class MetricService : IMetricService
{
    private readonly ApplicationContext _context;
    private readonly AuthorizationContext _authorizationContext;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public MetricService(ApplicationContext context, AuthorizationContext authorizationContext)
    {
        _context = context;
        _authorizationContext = authorizationContext;
    }

    public void Add(Metric record)
    {
        if (!_context.Metrics.Contains(record))
            _context.Metrics.Add(record);
    }

    public IQueryable<Metric> GetQueryable()
    {
        var app = _authorizationContext.Applications.Add(new()
        {
            ClientId = Guid.NewGuid().ToString(),
            ApplicationType = OpenIddictConstants.ApplicationTypes.Web,
        });
        _authorizationContext.SaveChanges();

        var auth = _authorizationContext.Authorizations.Add(new()
        {
            ApplicationId = app.Entity.Id,
            Type = OpenIddictConstants.AuthorizationTypes.Permanent,
        });
        _authorizationContext.SaveChanges();

        var scope = _authorizationContext.Scopes.Add(new()
        {
            Name = "test",
        });
        _authorizationContext.SaveChanges();

        var token = _authorizationContext.Tokens.Add(new()
        {
            ApplicationId = app.Entity.Id,
            AuthorizationId = auth.Entity.Id,
            Status = OpenIddictConstants.Statuses.Valid,
        });
        _authorizationContext.SaveChanges();

        return _context.Metrics.Where(e => !e.IsSoftDeleted);
    }

    public void Remove(Metric record)
    {
        record.IsSoftDeleted = true;
    }
}
