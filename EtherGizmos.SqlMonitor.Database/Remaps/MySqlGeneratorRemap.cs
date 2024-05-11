using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.MySql;
using System.Text.RegularExpressions;

namespace EtherGizmos.SqlMonitor.Database.Remaps;

internal class MySqlGeneratorRemap : MySql8Generator
{
    public MySqlGeneratorRemap() : base(new MySqlQuoterRemap()) { }

    public override string Generate(AlterColumnExpression expression)
    {
        var query = base.Generate(expression);
        query = new Regex(@"(`[^`]+`) CHAR\(36\)").Replace(query, "$1 BINARY(16)");

        return query;
    }

    public override string Generate(CreateColumnExpression expression)
    {
        var query = base.Generate(expression);
        query = new Regex(@"(`[^`]+`) CHAR\(36\)").Replace(query, "$1 BINARY(16)");

        return query;
    }

    public override string Generate(CreateTableExpression expression)
    {
        var query = base.Generate(expression);
        query = new Regex(@"(`[^`]+`) CHAR\(36\)").Replace(query, "$1 BINARY(16)");

        return query;
    }
}
