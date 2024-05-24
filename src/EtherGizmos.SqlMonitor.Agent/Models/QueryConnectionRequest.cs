namespace EtherGizmos.SqlMonitor.Agent.Models;

internal class QueryConnectionRequest
{
    public string Token { get; set; }

    public QueryConnectionRequest(string token)
    {
        Token = token;
    }
}
