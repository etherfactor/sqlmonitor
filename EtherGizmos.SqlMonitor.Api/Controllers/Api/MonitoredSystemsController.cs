using Asp.Versioning;
using AutoMapper;
using EtherGizmos.SqlMonitor.Api.Extensions;
using EtherGizmos.SqlMonitor.Api.Services.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Api.Services.Data.Abstractions;
using EtherGizmos.SqlMonitor.Models.Api.v1;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.OData.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Controllers.Api;

/// <summary>
/// Provides endpoints for <see cref="MonitoredSystem"/> records.
/// </summary>
public class MonitoredSystemsController : ODataController
{
    private const string BasePath = "api/v{version:apiVersion}/monitoredSystems";

    private readonly ILogger _logger;
    private readonly IDistributedRecordCache _cache;
    private readonly IMapper _mapper;
    private readonly IMonitoredSystemService _monitoredSystemService;
    private readonly ISaveService _saveService;

    /// <summary>
    /// Queries stored records.
    /// </summary>
    private IQueryable<MonitoredSystem> MonitoredSystems => _monitoredSystemService.GetQueryable();

    /// <summary>
    /// Constructs the controller.
    /// </summary>
    /// <param name="logger">The logger to utilize.</param>
    /// <param name="mapper">Allows conversion between database and DTO models.</param>
    /// <param name="instanceService">Provides access to the storage of records.</param>
    /// <param name="saveService">Provides access to saving records.</param>
    public MonitoredSystemsController(
        ILogger<MonitoredSystemsController> logger,
        IDistributedRecordCache cache,
        IMapper mapper,
        IMonitoredSystemService instanceService,
        ISaveService saveService)
    {
        _logger = logger;
        _cache = cache;
        _mapper = mapper;
        _monitoredSystemService = instanceService;
        _saveService = saveService;
    }

    /// <summary>
    /// Searches all records.
    /// </summary>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [ApiVersion("0.1")]
    [HttpGet(BasePath)]
    public async Task<IActionResult> Search(ODataQueryOptions<MonitoredSystemDTO> queryOptions)
    {
        var finished = await MonitoredSystems.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
        return Ok(finished);
    }

    /// <summary>
    /// Finds a single record.
    /// </summary>
    /// <param name="id">The id of the record.</param>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [ApiVersion("0.1")]
    [HttpGet(BasePath + "({id})")]
    public async Task<IActionResult> Get(Guid id, ODataQueryOptions<MonitoredSystemDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        MonitoredSystem? record = await MonitoredSystems.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<MonitoredSystemDTO>((e => e.Id, id)).GetResponse();

        var finished = record.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
        return Ok(finished);
    }

    /// <summary>
    /// Creates a record.
    /// </summary>
    /// <param name="newRecord">The record to create.</param>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [ApiVersion("0.1")]
    [HttpPost(BasePath)]
    public async Task<IActionResult> Create([FromBody] MonitoredSystemDTO newRecord, ODataQueryOptions<MonitoredSystemDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        await newRecord.EnsureValid(MonitoredSystems);

        MonitoredSystem record = _mapper.Map<MonitoredSystem>(newRecord);

        await record.EnsureValid(MonitoredSystems);
        _monitoredSystemService.Add(record);

        await _saveService.SaveChangesAsync();

        var finished = record.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
        return Created(finished);
    }

    /// <summary>
    /// Modifies a record.
    /// </summary>
    /// <param name="id">The id of the record to modify.</param>
    /// <param name="patchRecord">The delta patch to apply.</param>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [ApiVersion("0.1")]
    [HttpPatch(BasePath + "({id})")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Delta<MonitoredSystemDTO> patchRecord, ODataQueryOptions<MonitoredSystemDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        var testRecord = new MonitoredSystemDTO();
        patchRecord.Patch(testRecord);

        await testRecord.EnsureValid(MonitoredSystems);

        MonitoredSystem? record = await MonitoredSystems.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<MonitoredSystemDTO>((e => e.Id, id)).GetResponse();

        var recordAsDto = _mapper.MapExplicitly(record).To<MonitoredSystemDTO>();
        patchRecord.Patch(recordAsDto);

        _mapper.MergeInto(record).Using(recordAsDto);

        await record.EnsureValid(MonitoredSystems);

        await _saveService.SaveChangesAsync();
        await _cache.EntitySet<MonitoredSystem>().AddAsync(record);

        var finished = record.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
        return Ok(finished);
    }

    /// <summary>
    /// Soft-deletes a record.
    /// </summary>
    /// <param name="id">The id of the record to delete.</param>
    /// <returns>An awaitable task.</returns>
    [ApiVersion("0.1")]
    [HttpDelete(BasePath + "({id})")]
    public async Task<IActionResult> Delete(Guid id)
    {
        MonitoredSystem? record = await MonitoredSystems.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<MonitoredSystemDTO>((e => e.Id, id)).GetResponse();

        _monitoredSystemService.Remove(record);

        await _saveService.SaveChangesAsync();
        await _cache.EntitySet<MonitoredSystem>().RemoveAsync(record);

        return NoContent();
    }
}
