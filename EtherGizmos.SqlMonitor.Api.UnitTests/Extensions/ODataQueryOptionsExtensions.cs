using EtherGizmos.SqlMonitor.Api.Extensions;
using Microsoft.AspNetCore.OData.Query;

namespace EtherGizmos.SqlMonitor.Api.UnitTests.Extensions;

internal class ODataQueryOptionsExtensions
{
    [Test]
    public void EnsureValidForSingle_WhenNull_ThrowsArgumentNullException()
    {
        ODataQueryOptions<object>? options = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            options!.EnsureValidForSingle();
        });
    }
}
