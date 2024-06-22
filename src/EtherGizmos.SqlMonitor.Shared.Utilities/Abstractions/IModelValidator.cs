namespace EtherGizmos.SqlMonitor.Shared.Utilities.Abstractions;

public interface IModelValidator<TModel>
{
    public Task ValidateAsync(TModel model);
}
