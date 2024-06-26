﻿using Asp.Versioning;
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
/// Provides endpoints for <see cref="Metric"/> records.
/// </summary>
public class MetricsController : ODataController
{
    private const string BasePath = "api/v{version:apiVersion}/metrics";

    private readonly ILogger _logger;
    private readonly IRecordCache _cache;
    private readonly IMapper _mapper;
    private readonly IMetricService _metricService;
    private readonly IModelValidatorFactory _modelValidatorFactory;
    private readonly ISaveService _saveService;

    /// <summary>
    /// Queries stored records.
    /// </summary>
    private IQueryable<Metric> Metrics => _metricService.GetQueryable();

    /// <summary>
    /// Constructs the controller.
    /// </summary>
    /// <param name="logger">The logger to utilize.</param>
    /// <param name="mapper">Allows conversion between database and DTO models.</param>
    /// <param name="metricService">Provides access to the storage of records.</param>
    /// <param name="saveService">Provides access to saving records.</param>
    public MetricsController(
        ILogger<MetricsController> logger,
        IRecordCache cache,
        IMapper mapper,
        IMetricService metricService,
        IModelValidatorFactory modelValidatorFactory,
        ISaveService saveService)
    {
        _logger = logger;
        _cache = cache;
        _mapper = mapper;
        _metricService = metricService;
        _modelValidatorFactory = modelValidatorFactory;
        _saveService = saveService;
    }

    /// <summary>
    /// Searches all records.
    /// </summary>
    /// <param name="queryOptions">The query options to use.</param>
    /// <returns>An awaitable task.</returns>
    [ApiVersion("0.1")]
    [HttpGet(BasePath)]
    public async Task<IActionResult> Search(ODataQueryOptions<MetricDTO> queryOptions)
    {
        var finished = await Metrics.MapExplicitlyAndApplyQueryOptions(_mapper, queryOptions);
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
    public async Task<IActionResult> Get(int id, ODataQueryOptions<MetricDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        Metric? record = await Metrics.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<MetricDTO>((e => e.Id, id)).GetResponse();

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
    public async Task<IActionResult> Create([FromBody] MetricDTO newRecord, ODataQueryOptions<MetricDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        var validator = _modelValidatorFactory.GetValidator<MetricDTO>();
        await validator.ValidateAsync(newRecord);

        Metric record = _mapper.Map<Metric>(newRecord);

        var dbValidator = _modelValidatorFactory.GetValidator<Metric>();
        await dbValidator.ValidateAsync(record);

        _metricService.Add(record);

        await _saveService.SaveChangesAsync();
        await _cache.EntitySet<Metric>().AddAsync(record);

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
    public async Task<IActionResult> Update(int id, [FromBody] Delta<MetricDTO> patchRecord, ODataQueryOptions<MetricDTO> queryOptions)
    {
        queryOptions.EnsureValidForSingle();

        var testRecord = new MetricDTO();
        patchRecord.Patch(testRecord);

        var validator = _modelValidatorFactory.GetValidator<MetricDTO>();
        await validator.ValidateAsync(testRecord);

        Metric? record = await Metrics.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<MetricDTO>((e => e.Id, id)).GetResponse();

        var recordAsDto = _mapper.MapExplicitly(record).To<MetricDTO>();
        patchRecord.Patch(recordAsDto);

        _mapper.MergeInto(record).Using(recordAsDto);

        var dbValidator = _modelValidatorFactory.GetValidator<Metric>();
        await dbValidator.ValidateAsync(record);

        await _saveService.SaveChangesAsync();
        await _cache.EntitySet<Metric>().AddAsync(record);

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
        Metric? record = await Metrics.SingleOrDefaultAsync(e => e.Id == id);
        if (record == null)
            return new ODataRecordNotFoundError<MetricDTO>((e => e.Id, id)).GetResponse();

        _metricService.Remove(record);

        await _saveService.SaveChangesAsync();
        await _cache.EntitySet<Metric>().RemoveAsync(record);

        return NoContent();
    }
}
