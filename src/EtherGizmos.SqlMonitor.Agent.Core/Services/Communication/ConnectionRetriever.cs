using EtherGizmos.SqlMonitor.Agent.Core.Services.Communication.Abstractions;
using EtherGizmos.SqlMonitor.Shared.Models.Communication;
using EtherGizmos.SqlMonitor.Shared.Utilities.Extensions;
using System.Net.Http.Json;
using System.Text.Json;

namespace EtherGizmos.SqlMonitor.Agent.Core.Services.Communication;

public class ConnectionRetriever : IConnectionRetriever
{
    private readonly IHttpClientFactory _clientFactory;

    public ConnectionRetriever(
        IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<string> GetConnectionStringAsync(string connectionToken, CancellationToken cancellationToken = default)
    {
        var response = await ExchangeConnectionTokenAsync(connectionToken, cancellationToken);

        var result = JsonSerializer.Deserialize<DatabaseConfiguration>(response, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            ?? throw new InvalidOperationException("Unable to parse as JSON");

        return result.ConnectionString
            ?? throw new InvalidOperationException("Failed to parse a connection string");
    }

    public async Task<SshConfiguration> GetSshConfigurationAsync(string connectionToken, CancellationToken cancellationToken = default)
    {
        var response = await ExchangeConnectionTokenAsync(connectionToken, cancellationToken);

        var result = JsonSerializer.Deserialize<SshConfiguration>(response, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            ?? throw new InvalidOperationException("Unable to parse as JSON");

        return result;
    }

    public async Task<WinRmConfiguration> GetWinRmConfigurationAsync(string connectionToken, CancellationToken cancellationToken = default)
    {
        var response = await ExchangeConnectionTokenAsync(connectionToken, cancellationToken);

        var result = JsonSerializer.Deserialize<WinRmConfiguration>(response, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            ?? throw new InvalidOperationException("Unable to parse as JSON");

        return result;
    }

    private async Task<string> ExchangeConnectionTokenAsync(string connectionToken, CancellationToken cancellationToken = default)
    {
        using var client = _clientFactory.CreateClient("coordinator");

        var uriBuilder = new UriBuilder(client.BaseAddress!);
        uriBuilder.Path = $"/agent/v0.1/credentials";
        var queryParams = uriBuilder.GetQueryParameters();

        queryParams.Add("connectionToken", connectionToken);
        uriBuilder.SetQueryParameters(queryParams);

        var request = JsonContent.Create(new { });
        var httpResponse = await client.PostAsync(uriBuilder.Uri, request, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        var response = await httpResponse.Content.ReadAsStringAsync()
            ?? throw new InvalidOperationException("Did not receive any response content");

        return response;
    }
}
