using EtherGizmos.SqlMonitor.Api.OData.Abstractions;
using EtherGizmos.SqlMonitor.Api.OData.Metadata;
using Microsoft.OData.Edm;

namespace EtherGizmos.SqlMonitor.Api.OData;

public class VersionedODataModelProvider : IODataModelProvider
{
    private IDictionary<decimal, IEdmModel> CachedModels { get; } = new Dictionary<decimal, IEdmModel>();

    public IEdmModel GetEdmModel(decimal apiVersion)
    {
        if (CachedModels.TryGetValue(apiVersion, out IEdmModel? model))
            return model;

        model = ODataModel.GetEdmModel(apiVersion);
        CachedModels.Add(apiVersion, model);

        return model;
    }
}
