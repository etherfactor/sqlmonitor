using EtherGizmos.SqlMonitor.Shared.Configuration.Helpers;

namespace EtherGizmos.SqlMonitor.Shared.Configuration.Sources;

public class ImportFileOptions : IValidatableOptions
{
    public string Path { get; set; } = null!;

    public void AssertValid(string rootPath)
    {
        if (string.IsNullOrWhiteSpace(Path))
            ThrowHelper.ForMissingConfiguration(rootPath, this, nameof(Path), typeof(string));
    }
}
