using EtherGizmos.SqlMonitor.Shared.Configuration.Helpers;

namespace EtherGizmos.SqlMonitor.Shared.Configuration.Sources;

public class ImportOptions : IValidatableOptions
{
    public string? Name { get; set; }

    public int Priority { get; set; } = 1;

    public bool Optional { get; set; } = false;

    public ImportType Type { get; set; }

    #region File
    public ImportFileOptions File { get; set; } = new();
    #endregion File

    #region Database
    public ImportMySqlOptions MySql { get; set; } = new();

    public ImportPostgreSqlOptions PostgreSql { get; set; } = new();

    public ImportSqlServerOptions SqlServer { get; set; } = new();
    #endregion Database

    public void AssertValid(string rootPath)
    {
        if (Type == ImportType.Unknown)
            ThrowHelper.ForInvalidConfiguration(rootPath, this, e => e.Type, $"be one of '{string.Join("', '", Enum.GetValues<ImportType>().Where(e => e != ImportType.Unknown))}'");
        if (Type == ImportType.File)
            File.AssertValid($"{rootPath}:{nameof(File)}");
        if (Type == ImportType.MySql)
            MySql.AssertValid($"{rootPath}:{nameof(MySql)}");
        if (Type == ImportType.PostgreSql)
            PostgreSql.AssertValid($"{rootPath}:{nameof(PostgreSql)}");
        if (Type == ImportType.SqlServer)
            SqlServer.AssertValid($"{rootPath}:{nameof(SqlServer)}");
    }
}
