using System.Net.Http.Json;
using System.Text.Json;

namespace EtherGizmos.SqlMonitor.Shared.IntegrationTests.Extensions;

internal static class ObjectExtensions
{
    internal static JsonContent AsJsonContent<TEntity>(this TEntity @this)
    {
        var body = JsonContent.Create(@this);
        return body;
    }

    internal static StringContent AsStringContent<TEntity>(this TEntity @this)
    {
        var body = JsonSerializer.Serialize(@this);
        return new StringContent(body);
    }
}
