using EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.SqlMonitor.Shared.Utilities.Services;

internal class ModelValidatorFactory : IModelValidatorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ModelValidatorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IModelValidator<TModel> GetValidator<TModel>()
    {
        var validator = _serviceProvider.GetService<IModelValidator<TModel>>()
            ?? new NullModelValidator<TModel>();

        return validator;
    }

    private class NullModelValidator<TModel> : IModelValidator<TModel>
    {
        public Task ValidateAsync(TModel model)
        {
            return Task.CompletedTask;
        }
    }
}
