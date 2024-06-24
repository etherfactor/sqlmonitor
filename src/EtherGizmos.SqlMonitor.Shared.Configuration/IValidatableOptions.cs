namespace EtherGizmos.SqlMonitor.Shared.Configuration;

public interface IValidatableOptions
{
    void AssertValid(string rootPath);
}
