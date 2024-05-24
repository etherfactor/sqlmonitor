namespace EtherGizmos.SqlMonitor.Configuration;

public interface IValidatableOptions
{
    void AssertValid(string rootPath);
}
