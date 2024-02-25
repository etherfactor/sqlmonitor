using EtherGizmos.SqlMonitor.Api.Helpers;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Configuration;

public class SqlServerOptions
{
    public string DataSource { get; set; } = null!;

    public string InitialCatalog { get; set; } = null!;

    public bool TrustServerCertificate { get; set; }

    public bool IntegratedSecurity { get; set; }

    public Dictionary<string, string> AdditionalProperties { get; set; } = new();

    public void AssertValid(string rootPath)
    {
        if (DataSource is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, e => e.DataSource);
        if (InitialCatalog is null)
            ThrowHelper.ForMissingConfiguration(rootPath, this, e => e.InitialCatalog);
    }
}
