using EtherGizmos.SqlMonitor.Shared.Models.Api.v1;
using EtherGizmos.SqlMonitor.Shared.OData.Errors;
using EtherGizmos.SqlMonitor.Shared.OData.Exceptions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Shared.OData.Validators;

internal class QueryDTOValidator : IModelValidator<QueryDTO>
{
    public Task ValidateAsync(QueryDTO model)
    {
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

        return Task.CompletedTask;
    }
}
