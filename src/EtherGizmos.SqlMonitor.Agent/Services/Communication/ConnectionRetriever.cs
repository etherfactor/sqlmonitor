using EtherGizmos.SqlMonitor.Agent.Models;
using EtherGizmos.SqlMonitor.Agent.Services.Communication.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Utilities.Extensions;
using System.Net.Http.Json;

namespace EtherGizmos.SqlMonitor.Agent.Services.Communication;

internal class ConnectionRetriever : IConnectionRetriever
{
    private readonly IHttpClientFactory _clientFactory;

    public ConnectionRetriever(
        IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<string> GetConnectionStringAsync(string connectionToken, CancellationToken cancellationToken = default)
    {
        using var client = _clientFactory.CreateClient("coordinator");

        var uriBuilder = new UriBuilder(client.BaseAddress!);
        uriBuilder.Path = $"/agent/v0.1/credentials";
        var queryParams = uriBuilder.GetQueryParameters();

        queryParams.Add("connectionToken", connectionToken);
        uriBuilder.SetQueryParameters(queryParams);

        var request = JsonContent.Create(new QueryConnectionRequest(connectionToken));
        var httpResponse = await client.PostAsync(uriBuilder.Uri, request, cancellationToken);
        var response = await httpResponse.Content.ReadFromJsonAsync<QueryConnectionResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Unable to parse as JSON");

        return response.Value;
    }
}
