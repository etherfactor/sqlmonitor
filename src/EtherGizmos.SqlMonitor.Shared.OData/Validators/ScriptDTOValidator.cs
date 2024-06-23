using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1;
using EtherGizmos.SqlMonitor.Shared.OData.Errors;
using EtherGizmos.SqlMonitor.Shared.OData.Exceptions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Shared.OData.Validators;

internal class ScriptDTOValidator : IModelValidator<ScriptDTO>
{
    private readonly IMetricService _metricService;
    private readonly IScriptInterpreterService _scriptInterpreterService;

    public ScriptDTOValidator(
        IMetricService metricService,
        IScriptInterpreterService scriptInterpreterService)
    {
        _metricService = metricService;
        _scriptInterpreterService = scriptInterpreterService;
    }

    public async Task ValidateAsync(ScriptDTO model)
    {
        var distinctInterpreters = model.Variants
            .Select(e => e.ScriptInterpreterId)
            .Distinct();

        foreach (var scriptInterpreterId in distinctInterpreters)
        {
            var maybeInterpreter = await _scriptInterpreterService
                .GetQueryable()
                .SingleOrDefaultAsync(e => e.Id == scriptInterpreterId);

            if (maybeInterpreter is null)
            {
                var error = new ODataRecordNotFoundError<ScriptInterpreterDTO>((e => e.Id, scriptInterpreterId));
                throw new ReturnODataErrorException(error);
            }
        }

        var distinctMetrics = model.Metrics
            .Select(e => e.MetricId)
            .Distinct();

        foreach (var metricId in distinctMetrics)
        {
            var maybeMetric = await _metricService
                .GetQueryable()
                .SingleOrDefaultAsync(e => e.Id == metricId);

            if (maybeMetric is null)
            {
                var error = new ODataRecordNotFoundError<MetricDTO>((e => e.Id, metricId));
                throw new ReturnODataErrorException(error);
            }
        }

        var duplicates = model.Metrics.Select((e, i) => new { Index = i, Value = e })
            .GroupBy(e => e.Value.MetricId)
            .Where(e => e.Count() > 1);

        if (duplicates.Any())
        {
            var values = duplicates.SelectMany(e =>
                e.Take(1).Select(v => new { First = true, Value = v }).Concat(
                    e.Skip(1).Select(v => new { First = false, Value = v })));

            var error = new ODataDuplicateReferenceError<ScriptDTO>(values.Select(v =>
            {
                Expression<Func<ScriptDTO, object?>> expression = e => e.Metrics[v.Value.Index].MetricId;
                return (v.First, expression);
            }).ToArray());

            throw new ReturnODataErrorException(error);
        }
    }
}
