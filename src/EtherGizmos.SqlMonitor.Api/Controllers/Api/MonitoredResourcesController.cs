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
/// Provides endpoints for <see cref="MonitoredResource"/> records.
/// </summary>
public class MonitoredResourcesController : ODataController
{
    private const string BasePath = "api/v{version:apiVersion}/monitoredResources";

    private readonly ILogger _logger;
    private readonly IRecordCache _cache;
    private readonly IMapper _mapper;
    private readonly IModelValidatorFactory _modelValidatorFactory;
    private readonly IMonitoredResourceService _monitoredResourceService;
    private readonly ISaveService _saveService;

    /// <summary>
    /// Queries stored records.
    /// </summary>
    private IQueryable<MonitoredResource> MonitoredResources => _monitoredResourceService.GetQueryable();

    /// <summary>
    /// Constructs the controller.
    /// </summary>
    /// <param name="logger">The logger to utilize.</param>
    /// <param name="mapper">Allows conversion between database and DTO models.</param>
    /// <param name="resourceService">Provides access to the storage of records.</param>
    /// <param name="saveService">Provides access to saving records.</param>
    public MonitoredResourcesController(
        ILogger<MonitoredResourcesController> logger,
        IRecordCache cache,
        IMapper mapper,
        IModelValidatorFactory modelValidatorFactory,
        IMonitoredResourceService resourceService,
        ISaveService saveService)
    {
        _logger = logger;
        _cache = cache;
        _mapper = mapper;
        _modelValidatorFactory = modelValidatorFactory;
        _monitoredResourceService = resourceService;
        _saveService = saveService;
    }

    /// <summary>
    /// Searches all records.
    /// </summary>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [ApiVersion("0.1")]
    [HttpGet(BasePath)]
    public async Task<IActionResult> Search(ODataQueryOptions<MonitoredResourceDTO> queryOptions)
    {
        var finished = await MonitoredResources.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
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
    public async Task<IActionResult> Get(Guid id, ODataQueryOptions<MonitoredResourceDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        MonitoredResource? record = await MonitoredResources.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<MonitoredResourceDTO>((e => e.Id, id)).GetResponse();

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
    public async Task<IActionResult> Create([FromBody] MonitoredResourceDTO newRecord, ODataQueryOptions<MonitoredResourceDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        var validator = _modelValidatorFactory.GetValidator<MonitoredResourceDTO>();
        await validator.ValidateAsync(newRecord);

        MonitoredResource record = _mapper.Map<MonitoredResource>(newRecord);

        var dbValidator = _modelValidatorFactory.GetValidator<MonitoredResource>();
        await dbValidator.ValidateAsync(record);

        _monitoredResourceService.Add(record);

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
    public async Task<IActionResult> Update(Guid id, [FromBody] Delta<MonitoredResourceDTO> patchRecord, ODataQueryOptions<MonitoredResourceDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        var testRecord = new MonitoredResourceDTO();
        patchRecord.Patch(testRecord);

        var validator = _modelValidatorFactory.GetValidator<MonitoredResourceDTO>();
        await validator.ValidateAsync(testRecord);

        MonitoredResource? record = await MonitoredResources.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<MonitoredResourceDTO>((e => e.Id, id)).GetResponse();

        var recordAsDto = _mapper.MapExplicitly(record).To<MonitoredResourceDTO>();
        patchRecord.Patch(recordAsDto);

        _mapper.MergeInto(record).Using(recordAsDto);

        var dbValidator = _modelValidatorFactory.GetValidator<MonitoredResource>();
        await dbValidator.ValidateAsync(record);

        await _saveService.SaveChangesAsync();
        await _cache.EntitySet<MonitoredResource>().AddAsync(record);

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
        MonitoredResource? record = await MonitoredResources.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<MonitoredResourceDTO>((e => e.Id, id)).GetResponse();

        _monitoredResourceService.Remove(record);

        await _saveService.SaveChangesAsync();
        await _cache.EntitySet<MonitoredResource>().RemoveAsync(record);

        return NoContent();
    }
}
