using EtherGizmos.SqlMonitor.Shared.OData.Services;
using EtherGizmos.SqlMonitor.Shared.OData.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EtherGizmos.SqlMonitor.Shared.OData;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddModelValidators(this IServiceCollection @this)
    {
        @this.TryAddScoped<IModelValidatorFactory, ModelValidatorFactory>();

        var modelValidators = typeof(IServiceCollectionExtensions).Assembly
            .GetTypes()
            .Where(t => !t.IsInterface && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IModelValidator<>))
                .Select(i => new { ValidatorType = t, ModelType = i.GetGenericArguments().Single() }));

        foreach (var modelValidator in modelValidators)
        {
            var serviceType = typeof(IModelValidator<>).MakeGenericType(modelValidator.ModelType);
            @this.TryAddScoped(serviceType, modelValidator.ValidatorType);
        }

        return @this;
    }
}
