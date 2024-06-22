using Asp.Versioning;
using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Extensions;
using EtherGizmos.SqlMonitor.Shared.OData.Errors;
using EtherGizmos.SqlMonitor.Shared.OData.Extensions;
using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Controllers.Api;

/// <summary>
/// Provides endpoints for <see cref="MonitoredQueryTarget"/> records.
/// </summary>
public class MonitoredQueryTargetsController : ODataController
{
    private const string BasePath = "api/v{version:apiVersion}/monitoredQueryTargets";

    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IModelValidatorFactory _modelValidatorFactory;
    private readonly IMonitoredQueryTargetService _monitoredQueryTargetService;
    private readonly IMonitoredTargetService _monitoredTargetService;
    private readonly IRecordCache _cache;
    private readonly ISaveService _saveService;

    /// <summary>
    /// Queries stored records.
    /// </summary>
    private IQueryable<MonitoredQueryTarget> MonitoredQueryTargets => _monitoredQueryTargetService.GetQueryable();

    /// <summary>
    /// Constructs the controller.
    /// </summary>
    /// <param name="logger">The logger to utilize.</param>
    /// <param name="mapper">Allows conversion between database and DTO models.</param>
    /// <param name="instanceService">Provides access to the storage of records.</param>
    /// <param name="saveService">Provides access to saving records.</param>
    public MonitoredQueryTargetsController(
        ILogger<MonitoredQueryTargetsController> logger,
        IMapper mapper,
        IModelValidatorFactory modelValidatorFactory,
        IMonitoredQueryTargetService instanceService,
        IMonitoredTargetService targetService,
        IRecordCache cache,
        ISaveService saveService)
    {
        _logger = logger;
        _mapper = mapper;
        _modelValidatorFactory = modelValidatorFactory;
        _monitoredQueryTargetService = instanceService;
        _monitoredTargetService = targetService;
        _cache = cache;
        _saveService = saveService;
    }

    /// <summary>
    /// Searches all records.
    /// </summary>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [ApiVersion("0.1")]
    [HttpGet(BasePath)]
    public async Task<IActionResult> Search(ODataQueryOptions<MonitoredQueryTargetDTO> queryOptions)
    {
        var finished = await MonitoredQueryTargets.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
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
    public async Task<IActionResult> Get(int id, ODataQueryOptions<MonitoredQueryTargetDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        MonitoredQueryTarget? record = await MonitoredQueryTargets.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<MonitoredQueryTargetDTO>((e => e.Id, id)).GetResponse();

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
    public async Task<IActionResult> Create([FromBody] MonitoredQueryTargetDTO newRecord, ODataQueryOptions<MonitoredQueryTargetDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        var validator = _modelValidatorFactory.GetValidator<MonitoredQueryTargetDTO>();
        await validator.ValidateAsync(newRecord);

        MonitoredQueryTarget record = _mapper.Map<MonitoredQueryTarget>(newRecord);

        var target = await _monitoredTargetService.GetOrCreateAsync(
            record.MonitoredTarget.MonitoredSystemId,
            record.MonitoredTarget.MonitoredResourceId,
            record.MonitoredTarget.MonitoredEnvironmentId);

        record.MonitoredTarget = target;

        var dbValidator = _modelValidatorFactory.GetValidator<MonitoredQueryTarget>();
        await dbValidator.ValidateAsync(record);

        _monitoredQueryTargetService.Add(record);

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
    public async Task<IActionResult> Update(int id, [FromBody] Delta<MonitoredQueryTargetDTO> patchRecord, ODataQueryOptions<MonitoredQueryTargetDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        var testRecord = new MonitoredQueryTargetDTO();
        patchRecord.Patch(testRecord);

        var validator = _modelValidatorFactory.GetValidator<MonitoredQueryTargetDTO>();
        await validator.ValidateAsync(testRecord);

        MonitoredQueryTarget? record = await MonitoredQueryTargets.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<MonitoredQueryTargetDTO>((e => e.Id, id)).GetResponse();

        var recordAsDto = _mapper.MapExplicitly(record).To<MonitoredQueryTargetDTO>();
        patchRecord.Patch(recordAsDto);

        _mapper.MergeInto(record).Using(recordAsDto);

        var target = await _monitoredTargetService.GetOrCreateAsync(
            record.MonitoredTarget.MonitoredSystemId,
            record.MonitoredTarget.MonitoredResourceId,
            record.MonitoredTarget.MonitoredEnvironmentId);

        record.MonitoredTarget = target;

        var dbValidator = _modelValidatorFactory.GetValidator<MonitoredQueryTarget>();
        await dbValidator.ValidateAsync(record);

        await _saveService.SaveChangesAsync();

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
    public async Task<IActionResult> Delete(int id)
    {
        MonitoredQueryTarget? record = await MonitoredQueryTargets.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<MonitoredQueryTargetDTO>((e => e.Id, id)).GetResponse();

        _monitoredQueryTargetService.Remove(record);

        await _saveService.SaveChangesAsync();
        await _cache.EntitySet<MonitoredQueryTarget>().RemoveAsync(record);

        return NoContent();
    }
}
