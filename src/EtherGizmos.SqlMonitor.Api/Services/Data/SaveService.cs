using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;

namespace EtherGizmos.SqlMonitor.Api.Services.Data;

/// <summary>
/// Saves changes made by any/all <see cref="IEditableQueryableService{T}"/>.
/// </summary>
public class SaveService : ISaveService
{
    private readonly ILogger _logger;
    internal readonly ApplicationContext _context;

    /// <summary>
    /// Construct the service.
    /// </summary>
    /// <param name="context">The EF context.</param>
    public SaveService(ILogger<SaveService> logger, ApplicationContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <inheritdoc/>
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public void Attach(object record)
    {
        _context.Attach(record);
    }
}
