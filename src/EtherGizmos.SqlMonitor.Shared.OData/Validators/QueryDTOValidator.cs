using EtherGizmos.SqlMonitor.Shared.Database.Services.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Api.v1;
using EtherGizmos.SqlMonitor.Shared.OData.Errors;
using EtherGizmos.SqlMonitor.Shared.OData.Exceptions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Shared.OData.Validators;

internal class QueryDTOValidator : IModelValidator<QueryDTO>
{
    private readonly IMetricService _metricService;

    public QueryDTOValidator(
        IMetricService metricService)
    {
        _metricService = metricService;
    }

    public async Task ValidateAsync(QueryDTO model)
    {
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

            var error = new ODataDuplicateReferenceError<QueryDTO>(values.Select(v =>
            {
                Expression<Func<QueryDTO, object?>> expression = e => e.Metrics[v.Value.Index].MetricId;
                return (v.First, expression);
            }).ToArray());

            throw new ReturnODataErrorException(error);
        }
    }
}
