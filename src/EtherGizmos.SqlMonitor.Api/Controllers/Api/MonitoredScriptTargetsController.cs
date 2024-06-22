using Asp.Versioning;
using AutoMapper;
using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1;
using EtherGizmos.SqlMonitor.Shared.Models.Database;
using EtherGizmos.SqlMonitor.Shared.Models.Extensions;
using EtherGizmos.SqlMonitor.Shared.OData.Errors;
using EtherGizmos.SqlMonitor.Shared.OData.Extensions;
using EtherGizmos.SqlMonitor.Shared.Redis.Caching.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Redis.Locking.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.SqlMonitor.Api.Controllers.Api;

/// <summary>
/// Provides endpoints for <see cref="MonitoredScriptTarget"/> records.
/// </summary>
public class MonitoredScriptTargetsController : ODataController
{
    private const string BasePath = "api/v{version:apiVersion}/monitoredScriptTargets";

    private readonly ILogger _logger;
    private readonly ILockingCoordinator _coordinator;
    private readonly IMapper _mapper;
    private readonly IModelValidatorFactory _modelValidatorFactory;
    private readonly IMonitoredScriptTargetService _monitoredScriptTargetService;
    private readonly IMonitoredTargetLockFactory _monitoredTargetLockFactory;
    private readonly IMonitoredTargetService _monitoredTargetService;
    private readonly IRecordCache _cache;
    private readonly ISaveService _saveService;

    /// <summary>
    /// Queries stored records.
    /// </summary>
    private IQueryable<MonitoredScriptTarget> MonitoredScriptTargets => _monitoredScriptTargetService.GetQueryable();

    /// <summary>
    /// Constructs the controller.
    /// </summary>
    /// <param name="logger">The logger to utilize.</param>
    /// <param name="mapper">Allows conversion between database and DTO models.</param>
    /// <param name="instanceService">Provides access to the storage of records.</param>
    /// <param name="saveService">Provides access to saving records.</param>
    public MonitoredScriptTargetsController(
        ILogger<MonitoredScriptTargetsController> logger,
        ILockingCoordinator coordinator,
        IMapper mapper,
        IModelValidatorFactory modelValidatorFactory,
        IMonitoredScriptTargetService instanceService,
        IMonitoredTargetLockFactory targetLockFactory,
        IMonitoredTargetService targetService,
        IRecordCache cache,
        ISaveService saveService)
    {
        _logger = logger;
        _coordinator = coordinator;
        _mapper = mapper;
        _modelValidatorFactory = modelValidatorFactory;
        _monitoredScriptTargetService = instanceService;
        _monitoredTargetLockFactory = targetLockFactory;
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
    public async Task<IActionResult> Search(ODataQueryOptions<MonitoredScriptTargetDTO> queryOptions)
    {
        var finished = await MonitoredScriptTargets.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
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
    public async Task<IActionResult> Get(int id, ODataQueryOptions<MonitoredScriptTargetDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        MonitoredScriptTarget? record = await MonitoredScriptTargets.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<MonitoredScriptTargetDTO>((e => e.Id, id)).GetResponse();

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
    public async Task<IActionResult> Create([FromBody] MonitoredScriptTargetDTO newRecord, ODataQueryOptions<MonitoredScriptTargetDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        var validator = _modelValidatorFactory.GetValidator<MonitoredScriptTargetDTO>();
        await validator.ValidateAsync(newRecord);

        MonitoredScriptTarget record = _mapper.Map<MonitoredScriptTarget>(newRecord);

        var target = await _monitoredTargetService.GetOrCreateAsync(
            record.MonitoredTarget.MonitoredSystemId,
            record.MonitoredTarget.MonitoredResourceId,
            record.MonitoredTarget.MonitoredEnvironmentId);

        record.MonitoredTargetId = target.Id;
        record.MonitoredTarget = null!;

        var dbValidator = _modelValidatorFactory.GetValidator<MonitoredScriptTarget>();
        await dbValidator.ValidateAsync(record);

        _monitoredScriptTargetService.Add(record);

        await _saveService.SaveChangesAsync();
        record.MonitoredTarget = target;

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
    public async Task<IActionResult> Update(int id, [FromBody] Delta<MonitoredScriptTargetDTO> patchRecord, ODataQueryOptions<MonitoredScriptTargetDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        var testRecord = new MonitoredScriptTargetDTO();
        patchRecord.Patch(testRecord);

        var validator = _modelValidatorFactory.GetValidator<MonitoredScriptTargetDTO>();
        await validator.ValidateAsync(testRecord);

        MonitoredScriptTarget? record = await MonitoredScriptTargets.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<MonitoredScriptTargetDTO>((e => e.Id, id)).GetResponse();

        var recordAsDto = _mapper.MapExplicitly(record).To<MonitoredScriptTargetDTO>();
        patchRecord.Patch(recordAsDto);

        _mapper.MergeInto(record).Using(recordAsDto);

        var target = await _monitoredTargetService.GetOrCreateAsync(
            record.MonitoredTarget.MonitoredSystemId,
            record.MonitoredTarget.MonitoredResourceId,
            record.MonitoredTarget.MonitoredEnvironmentId);

        record.MonitoredTargetId = target.Id;
        record.MonitoredTarget = null!;

        var dbValidator = _modelValidatorFactory.GetValidator<MonitoredScriptTarget>();
        await dbValidator.ValidateAsync(record);

        await _saveService.SaveChangesAsync();
        record.MonitoredTarget = target;

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
        MonitoredScriptTarget? record = await MonitoredScriptTargets.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<MonitoredScriptTargetDTO>((e => e.Id, id)).GetResponse();

        _monitoredScriptTargetService.Remove(record);

        await _saveService.SaveChangesAsync();
        await _cache.EntitySet<MonitoredScriptTarget>().RemoveAsync(record);

        return NoContent();
    }
}
