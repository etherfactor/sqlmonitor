using FluentMigrator;
using FluentMigrator.Runner.Generators.MySql;

namespace EtherGizmos.SqlMonitor.Database.Remaps;

public class MySqlQuoterRemap : MySqlQuoter
{
    public override string FormatSystemMethods(SystemMethods value)
    {
        switch (value)
        {
            case SystemMethods.CurrentUTCDateTime:
                return "(UTC_TIMESTAMP)";
        }

        return base.FormatSystemMethods(value);
    }
}
