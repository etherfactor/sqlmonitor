using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Provides access to <see cref="InstanceMetricBySecond"/> records.
/// </summary>
public class InstanceMetricBySecondService : IInstanceMetricBySecondService
{
    private readonly ILogger _logger;

    private readonly DatabaseContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <param name="context">Provides access to internal records.</param>
    /// <param name="distributedRecordCache">Contains shared, cached records.</param>
    public InstanceMetricBySecondService(
        ILogger<InstanceMetricBySecondService> logger,
        DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <inheritdoc/>
    public void Add(InstanceMetricBySecond record)
    {
        if (!_context.InstanceMetricsBySecond.Contains(record))
            _context.InstanceMetricsBySecond.Add(record);
    }

    /// <inheritdoc/>
    public IQueryable<InstanceMetricBySecond> GetQueryable()
    {
        return _context.InstanceMetricsBySecond;
    }

    /// <inheritdoc/>
    public void Remove(InstanceMetricBySecond record)
    {
        throw new NotSupportedException();
    }
}
