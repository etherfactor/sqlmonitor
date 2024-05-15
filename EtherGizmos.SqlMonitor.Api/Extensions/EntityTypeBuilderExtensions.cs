using EtherGizmos.SqlMonitor.Models.Annotations;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

/// <summary>
/// Provides extension methods for <see cref="EntityTypeBuilder{TEntity}"/>.
/// </summary>
internal static class EntityTypeBuilderExtensions
{
    /// <summary>
    /// Adds auditing columns to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>Itself.</returns>
    internal static EntityTypeBuilder<TEntity> AuditPropertiesWithAnnotations<TEntity>(this EntityTypeBuilder<TEntity> @this)
        where TEntity : class, IAuditable
    {
        @this.PropertyWithAnnotations(e => e.CreatedAt).HasDefaultValueSql();
        @this.PropertyWithAnnotations(e => e.CreatedByUserId);
        @this.PropertyWithAnnotations(e => e.ModifiedAt);
        @this.PropertyWithAnnotations(e => e.ModifiedByUserId);

        return @this;
    }

    /// <summary>
    /// Maps to a table, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <returns>Itself.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal static EntityTypeBuilder<TEntity> ToTableWithAnnotations<TEntity>(this EntityTypeBuilder<TEntity> @this, string? schema = null, Action<TableBuilder<TEntity>>? buildAction = null)
        where TEntity : class
    {
        //Extract the table name from a TableAttribute, and ensure it exists
        var attribute = typeof(TEntity).GetCustomAttribute<TableAttribute>();
        string tableName = attribute?.Name
            ?? throw new InvalidOperationException(string.Format("Type '{0}' must be annotated with a '{1}' and specify the '{2}' property",
                typeof(TEntity).Name, nameof(TableAttribute), nameof(TableAttribute.Name)));

        //Map to the table
        if (buildAction is not null)
        {
            @this.ToTable(tableName, schema, buildAction);
        }
        else
        {
            @this.ToTable(tableName, schema);
        }

        return @this;
    }

    /// <summary>
    /// Adds a property, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TMember">The type of entity member.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">An expression selecting the property.</param>
    /// <returns>The property builder.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal static PropertyBuilder<TMember> PropertyWithAnnotations<TEntity, TMember>(this EntityTypeBuilder<TEntity> @this, Expression<Func<TEntity, TMember>> propertyExpression)
        where TEntity : class
    {
        //Extract the property info from the expression
        var property = propertyExpression.GetPropertyInfo();

        //Use the overridden property, if it exists
        property = typeof(TEntity)
            .GetProperty(property.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?? property;

        //Create the property builder
        var builder = @this.Property(propertyExpression);

        //Extract the column name from a ColumnAttribute, and ensure it exists
        var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
        string columnName = columnAttribute?.Name
            ?? throw new InvalidOperationException(string.Format("Property '{0}' on type '{1}' must be annotated with a '{2}' and specify the '{3}' property",
                 property.Name, typeof(TEntity).Name, nameof(ColumnAttribute), nameof(ColumnAttribute.Name)));

        builder.HasColumnName(columnName);

        //Test for a SqlDefaultValueAttribute to determine if a default value is generated
        var sqlDefaultValueAttribute = property.GetCustomAttribute<SqlDefaultValueAttribute>();
        if (sqlDefaultValueAttribute != null)
            builder.HasDefaultValueSql();

        ////Should mitigate the issue described in https://github.com/dotnet/efcore/issues/6054
        ////Removed for now, but comment left in case the issue pops back up
        //if (typeof(TMember) == typeof(bool))
        //    builder.ValueGeneratedNever();

        return builder;
    }
}
