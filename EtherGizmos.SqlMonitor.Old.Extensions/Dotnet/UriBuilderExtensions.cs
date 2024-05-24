using System.Collections.Specialized;
using System.Web;

namespace EtherGizmos.SqlMonitor.Api.Extensions.Dotnet;

public static class UriBuilderExtensions
{
    public static NameValueCollection GetQueryParameters(this UriBuilder @this)
    {
        var queryParams = HttpUtility.ParseQueryString(@this.Query);
        return queryParams;
    }

    public static void SetQueryParameters(this UriBuilder @this, NameValueCollection queryParams)
    {
        var queryString = queryParams.ToString();
        @this.Query = queryString;
    }
}
