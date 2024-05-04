using System.Net.Http.Json;

namespace EtherGizmos.SqlMonitor.Shared.IntegrationTests.Extensions;

internal static class HttpContentExtensions
{
    public static async Task<TEntity?> ReadFromJsonModelAsync<TEntity>(this HttpContent @this, TEntity? model = default)
    {
        return await @this.ReadFromJsonAsync<TEntity>();
    }
}
