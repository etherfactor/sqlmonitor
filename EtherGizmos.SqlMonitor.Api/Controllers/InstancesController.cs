﻿using Asp.Versioning;
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

namespace EtherGizmos.SqlMonitor.Api.Controllers;

/// <summary>
/// Provides endpoints for <see cref="Instance"/> records.
/// </summary>
public class InstancesController : ODataController
{
    private const string BasePath = "api/v{version:apiVersion}/instances";

    private readonly ILogger _logger;
    private readonly IDistributedRecordCache _cache;
    private readonly IMapper _mapper;
    private readonly IInstanceService _instanceService;
    private readonly ISaveService _saveService;

    /// <summary>
    /// Queries stored records.
    /// </summary>
    private IQueryable<Instance> Instances => _instanceService.GetQueryable();

    /// <summary>
    /// Constructs the controller.
    /// </summary>
    /// <param name="logger">The logger to utilize.</param>
    /// <param name="mapper">Allows conversion between database and DTO models.</param>
    /// <param name="instanceService">Provides access to the storage of records.</param>
    /// <param name="saveService">Provides access to saving records.</param>
    public InstancesController(
        ILogger<InstancesController> logger,
        IDistributedRecordCache cache,
        IMapper mapper,
        IInstanceService instanceService,
        ISaveService saveService)
    {
        _logger = logger;
        _cache = cache;
        _mapper = mapper;
        _instanceService = instanceService;
        _saveService = saveService;
    }

    /// <summary>
    /// Searches all records.
    /// </summary>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [ApiVersion("0.1")]
    [HttpGet(BasePath)]
    public async Task<IActionResult> Search(ODataQueryOptions<InstanceDTO> queryOptions)
    {
        var finished = await Instances.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
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
    public async Task<IActionResult> Get(Guid id, ODataQueryOptions<InstanceDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        Instance? record = await Instances.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<InstanceDTO>((e => e.Id, id)).GetResponse();

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
    public async Task<IActionResult> Create([FromBody] InstanceDTO newRecord, ODataQueryOptions<InstanceDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        await newRecord.EnsureValid(Instances);

        Instance record = _mapper.Map<Instance>(newRecord);

        await record.EnsureValid(Instances);
        _instanceService.Add(record);

        await _saveService.SaveChangesAsync();
        await _cache.EntitySet<Instance>().AddAsync(record);

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
    public async Task<IActionResult> Update(Guid id, [FromBody] Delta<InstanceDTO> patchRecord, ODataQueryOptions<InstanceDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        var testRecord = new InstanceDTO();
        patchRecord.Patch(testRecord);

        await testRecord.EnsureValid(Instances);

        Instance? record = await Instances.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<InstanceDTO>((e => e.Id, id)).GetResponse();

        var recordAsDto = _mapper.MapExplicitly(record).To<InstanceDTO>();
        patchRecord.Patch(recordAsDto);

        _mapper.MergeInto(record).Using(recordAsDto);

        await record.EnsureValid(Instances);

        await _saveService.SaveChangesAsync();
        await _cache.EntitySet<Instance>().AddAsync(record);

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
        Instance? record = await Instances.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<InstanceDTO>((e => e.Id, id)).GetResponse();

        _instanceService.Remove(record);

        await _saveService.SaveChangesAsync();
        await _cache.EntitySet<Instance>().RemoveAsync(record);

        return NoContent();
    }
}
