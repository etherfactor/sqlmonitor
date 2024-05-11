using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using System.ComponentModel;
using System.Data;

namespace EtherGizmos.SqlMonitor.Database.Core;

//See https://github.com/fluentmigrator/fluentmigrator/issues/640

public abstract class MigrationExtension : Migration
{
    public IMergeExpressionRoot Merge
    {
        get
        {
            return new MergeExpressionRoot((IMigrationContext)GetType().GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(this)!);
        }
    }
}

public interface IMergeExpressionRoot
{
    IMergeDataOrInSchemaSyntax IntoTable(string tableName);
}

public class MergeExpressionRoot : IMergeExpressionRoot
{
    private readonly IMigrationContext _context;

    public MergeExpressionRoot(IMigrationContext context)
    {
        _context = context;
    }

    public IMergeDataOrInSchemaSyntax IntoTable(string tableName)
    {
        var expression = new MergeDataExpression { TableName = tableName };
        _context.Expressions.Add(expression);
        return new MergeDataExpressionStartBuilder(expression);
    }
}

public interface IMergeDataSyntax
{
    IMergeDataOrMatchSyntax<T> Row<T>(T dataAsAnonymousType);
}

public interface IMergeDataOrInSchemaSyntax : IMergeDataSyntax
{
    IMergeDataSyntax InSchema(string schemaName);
}

public interface IMergeDataOrMatchSyntax<T>
{
    IMergeDataOrMatchSyntax<T> Row(T dataAsAnonymousType);
    void Match<M>(Func<T, M> f);
}

public abstract class MergeDataExpressionBuilderBase : ISupportAdditionalFeatures
{
    protected readonly MergeDataExpression _expression;

    public IDictionary<string, object> AdditionalFeatures => _expression.AdditionalFeatures;

    protected MergeDataExpressionBuilderBase(MergeDataExpression expression)
    {
        _expression = expression;
    }

    protected static IDictionary<string, object> ExtractData(object dataAsAnonymousType)
    {
        var data = new Dictionary<string, object>();

        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(dataAsAnonymousType);

        foreach (PropertyDescriptor property in properties)
        {
            data.Add(property.Name, property.GetValue(dataAsAnonymousType)!);
        }

        return data;
    }
}

public class MergeDataExpressionStartBuilder : MergeDataExpressionBuilderBase, IMergeDataOrInSchemaSyntax
{
    public MergeDataExpressionStartBuilder(MergeDataExpression expression) : base(expression)
    {
    }

    public IMergeDataOrMatchSyntax<T> Row<T>(T dataAsAnonymousType)
    {
        var typed = new MergeDataExpressionTypedBuilder<T>(_expression);
        return typed.Row(dataAsAnonymousType);
    }

    public IMergeDataSyntax InSchema(string schemaName)
    {
        _expression.SchemaName = schemaName;
        return this;
    }
}

public class MergeDataExpressionTypedBuilder<T> : MergeDataExpressionBuilderBase, IMergeDataOrMatchSyntax<T>
{
    public MergeDataExpressionTypedBuilder(MergeDataExpression expression) : base(expression)
    {
    }

    public IMergeDataOrMatchSyntax<T> Row(T dataAsAnonymousType)
    {
        IDictionary<string, object> data = ExtractData(dataAsAnonymousType!);

        var dataDefinition = new InsertionDataDefinition();

        dataDefinition.AddRange(data);

        _expression.Rows.Add(dataDefinition);

        return this;
    }

    public void Match<M>(Func<T, M> f)
    {
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(M));
        foreach (PropertyDescriptor property in properties)
        {
            _expression.MatchColumns.Add(property.Name);
        }
    }
}

public class MergeDataExpression : MigrationExpressionBase
{
    private readonly List<InsertionDataDefinition> _rows = new List<InsertionDataDefinition>();

    public string SchemaName { get; set; } = null!;

    public string TableName { get; set; } = null!;

    private readonly Dictionary<string, object> _additionalFeatures = new Dictionary<string, object>();

    private readonly List<string> _matchColumns = new List<string>();

    public List<InsertionDataDefinition> Rows
    {
        get { return _rows; }
    }

    public IDictionary<string, object> AdditionalFeatures
    {
        get { return _additionalFeatures; }
    }

    public List<string> MatchColumns
    {
        get { return _matchColumns; }
    }

    public override void ExecuteWith(IMigrationProcessor processor)
    {
        var existingDataSet = processor.ReadTableData(SchemaName, TableName);
        var existingTable = existingDataSet.Tables[0];

        foreach (var row in _rows)
        {
            var exists = existingTable.Rows.OfType<DataRow>().Any(r =>
            {
                return _matchColumns.Select(mc =>
                {
                    var ex = r[mc];
                    var nw = row.Where(p => p.Key == mc).Select(p => p.Value).SingleOrDefault();
                    if (ex == null || nw == null)
                    {
                        return ex == nw;
                    }
                    return ex.Equals(nw);
                }).All(m => m);
            });

            if (exists)
            {
                ExecuteUpdateWith(processor, row);
            }
            else
            {
                ExecuteInsertWith(processor, row);
            }
        }
    }

    private void ExecuteUpdateWith(IMigrationProcessor processor, List<KeyValuePair<string, object>> row)
    {
        var update = new UpdateDataExpression
        {
            SchemaName = SchemaName,
            TableName = TableName,
            IsAllRows = false,
            Set = row.Where(p => !_matchColumns.Contains(p.Key)).ToList(),
            Where = _matchColumns.Select(mc =>
            {
                var v = row.Where(p => p.Key == mc).Select(p => p.Value).SingleOrDefault();
                return new KeyValuePair<string, object>(mc, v!);
            }).ToList()
        };

        processor.Process(update);
    }

    private void ExecuteInsertWith(IMigrationProcessor processor, InsertionDataDefinition row)
    {
        var insert = new InsertDataExpression
        {
            SchemaName = SchemaName,
            TableName = TableName
        };

        foreach (var af in _additionalFeatures)
        {
            insert.AdditionalFeatures.Add(af.Key, af.Value);
        }

        insert.Rows.Add(row);

        processor.Process(insert);
    }
}
