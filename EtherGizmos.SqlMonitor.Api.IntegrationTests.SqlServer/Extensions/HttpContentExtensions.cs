using System.Net.Http.Json;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests.SqlServer.Extensions;

internal static class HttpContentExtensions
{
    public static async Task<TEntity?> ReadFromJsonModelAsync<TEntity>(this HttpContent @this, TEntity? model = default)
    {
        return await @this.ReadFromJsonAsync<TEntity>();
    }
}
