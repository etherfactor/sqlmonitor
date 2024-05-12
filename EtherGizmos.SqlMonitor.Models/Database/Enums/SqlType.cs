namespace EtherGizmos.SqlMonitor.Models.Database.Enums;

public enum SqlType
{
    Unknown = 0,
    MicrosoftSqlServer = 10,
    MySql = 20,
    MariaDb = 25,
    PostgreSql = 30,
}

public static class SqlTypeConverter
{
    private static Dictionary<SqlType, int> Mappings { get; } = new Dictionary<SqlType, int>()
    {
        { SqlType.MicrosoftSqlServer, 10 },
        { SqlType.MySql, 20 },
        { SqlType.MariaDb, 30 },
        { SqlType.PostgreSql, 35 },
    };

    public static SqlType FromInteger(int value)
    {
        if (!Mappings.ContainsValue(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static SqlType? FromIntegerOrDefault(int? value)
    {
        if (value == null || !Mappings.ContainsValue((int)value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static int ToInteger(SqlType value)
    {
        if (value == SqlType.Unknown)
            return -1;

        if (!Mappings.ContainsKey(value))
            throw new InvalidOperationException(string.Format("Unmapped type: {0}", value));

        return Mappings.Single(e => e.Key == value).Value;
    }

    public static int? ToIntegerOrDefault(SqlType? value)
    {
        if (value == SqlType.Unknown)
            return -1;

        if (value == null || !Mappings.ContainsKey((SqlType)value))
            return null;

        return Mappings.Single(e => e.Key == value).Value;
    }
}
