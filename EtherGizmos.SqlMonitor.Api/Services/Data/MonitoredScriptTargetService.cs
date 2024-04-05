﻿using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides access to <see cref="MonitoredScriptTarget"/> records.
/// </summary>
public class MonitoredScriptTargetService : IMonitoredScriptTargetService
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The internal database context.</param>
    public MonitoredScriptTargetService(DatabaseContext context)
    {
        _context = context;
    }

    public void Add(MonitoredScriptTarget record)
    {
        if (!_context.MonitoredScriptTargets.Contains(record))
            _context.MonitoredScriptTargets.Add(record);
    }

    public IQueryable<MonitoredScriptTarget> GetQueryable()
    {
        return _context.MonitoredScriptTargets;
    }

    public void Remove(MonitoredScriptTarget record)
    {
        _context.MonitoredScriptTargets.Remove(record);
    }
}