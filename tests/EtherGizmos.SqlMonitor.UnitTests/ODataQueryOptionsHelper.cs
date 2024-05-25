namespace EtherGizmos.SqlMonitor.UnitTests;

internal static class ODataQueryOptionsHelper
{
    internal static ODataQueryOptions<TEntity> CreateOptions<TEntity>(IEdmModel model, string method, string root, string prefix, string entitySet, string entity, string queryOptions)
    {
        var request = RequestFactory.Create(method, $"{root}/{prefix}/{entitySet}{entity}?{queryOptions}", opt =>
        {
            opt.AddRouteComponents($"/{prefix}", model);
        });

        request.ODataFeature().Model = model;
        request.ODataFeature().Path = new ODataPath(new EntitySetSegment(model.EntityContainer.FindEntitySet(entitySet)));
        request.ODataFeature().RoutePrefix = $"/{prefix}";

        var oDataQueryContext = new ODataQueryContext(model, typeof(TEntity), new ODataPath());
        var oDataQueryOptions = new ODataQueryOptions<TEntity>(oDataQueryContext, request);

        return oDataQueryOptions;
    }
}
