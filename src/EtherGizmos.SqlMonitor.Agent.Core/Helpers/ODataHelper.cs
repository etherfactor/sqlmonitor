namespace EtherGizmos.SqlMonitor.Agent.Core.Helpers;

internal static class ODataHelper
{
    internal static string EscapeString(string? input)
    {
        if (input is null)
            return "''";

        return $"'{input.Replace("'", "''")}'";
    }
}
