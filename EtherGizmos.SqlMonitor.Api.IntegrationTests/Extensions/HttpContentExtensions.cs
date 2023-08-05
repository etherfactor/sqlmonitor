using System.Net.Http.Json;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.Extensions;

internal static class HttpContentExtensions
{
    public static async Task<TEntity?> ReadFromJsonModelAsync<TEntity>(this HttpContent @this, TEntity model)
    {
        return await @this.ReadFromJsonAsync<TEntity>();
    }
}
