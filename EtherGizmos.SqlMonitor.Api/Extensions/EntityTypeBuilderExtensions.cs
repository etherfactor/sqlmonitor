using EtherGizmos.SqlMonitor.Models.Database.Abstractions;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

internal static class EntityTypeBuilderExtensions
{
    internal static EntityTypeBuilder<TEntity> AuditPropertiesWithAnnotations<TEntity>(this EntityTypeBuilder<TEntity> @this)
        where TEntity : class, IAuditable
    {
        @this.PropertyWithAnnotations(e => e.CreatedAt);
        @this.PropertyWithAnnotations(e => e.CreatedByUserId);
        @this.PropertyWithAnnotations(e => e.ModifiedAt);
        @this.PropertyWithAnnotations(e => e.ModifiedByUserId);

        return @this;
    }

    internal static EntityTypeBuilder<TEntity> ToTableWithAnnotations<TEntity>(this EntityTypeBuilder<TEntity> @this)
        where TEntity : class
    {
        var attribute = typeof(TEntity).GetCustomAttribute<TableAttribute>();
        string tableName = attribute?.Name
            ?? throw new InvalidOperationException(string.Format("Type '{0}' must be annotated with a '{1}' and specify the '{2}' property",
                typeof(TEntity).Name, nameof(TableAttribute), nameof(TableAttribute.Name)));

        @this.ToTable(tableName);

        return @this;
    }

    internal static PropertyBuilder<TMember> PropertyWithAnnotations<TEntity, TMember>(this EntityTypeBuilder<TEntity> @this, Expression<Func<TEntity, TMember>> propertyExpression)
        where TEntity : class
    {
        var property = propertyExpression.GetPropertyInfo();
        var attribute = property.GetCustomAttribute<ColumnAttribute>();
        string columnName = attribute?.Name
            ?? throw new InvalidOperationException(string.Format("Property '{0}' on type '{1}' must be annotated with a '{2}' and specify the '{3}' property",
                 property.Name, typeof(TEntity).Name, nameof(ColumnAttribute), nameof(ColumnAttribute.Name)));

        var builder = @this.Property(propertyExpression);
        builder.HasColumnName(columnName);

        return builder;
    }
}
