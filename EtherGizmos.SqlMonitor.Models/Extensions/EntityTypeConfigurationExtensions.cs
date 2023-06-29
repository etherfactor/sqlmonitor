using EtherGizmos.SqlMonitor.Models.Api.Abstractions;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EtherGizmos.SqlMonitor.Models.Extensions;

internal static class EntityTypeConfigurationExtensions
{
    internal static void AuditPropertiesWithAnnotations<TEntity>(this EntityTypeConfiguration<TEntity> @this)
        where TEntity : class, IAuditableDTO
    {
        @this.PropertyWithAnnotations(e => e.CreatedAt);
        @this.PropertyWithAnnotations(e => e.CreatedByUserId);
        @this.PropertyWithAnnotations(e => e.ModifiedAt);
        @this.PropertyWithAnnotations(e => e.ModifiedByUserId);
    }

    internal static NavigationPropertyConfiguration HasManyWithAnnotations<TEntity, TMember>(this EntityTypeConfiguration<TEntity> @this, Expression<Func<TEntity, IEnumerable<TMember>>> navigationPropertyExpression)
        where TEntity : class
        where TMember : class
    {
        var configuration = @this.HasMany(navigationPropertyExpression);
        configuration.LoadNameConfiguration(navigationPropertyExpression);

        return configuration;
    }

    internal static NavigationPropertyConfiguration HasOptionalWithAnnotations<TEntity, TMember>(this EntityTypeConfiguration<TEntity> @this, Expression<Func<TEntity, TMember>> navigationPropertyExpression)
        where TEntity : class
        where TMember : class
    {
        var configuration = @this.HasOptional(navigationPropertyExpression);
        configuration.LoadNameConfiguration(navigationPropertyExpression);

        return configuration;
    }

    internal static NavigationPropertyConfiguration HasRequiredWithAnnotations<TEntity, TMember>(this EntityTypeConfiguration<TEntity> @this, Expression<Func<TEntity, TMember>> navigationPropertyExpression)
        where TEntity : class
        where TMember : class
    {
        var configuration = @this.HasRequired(navigationPropertyExpression);
        configuration.LoadNameConfiguration(navigationPropertyExpression);

        return configuration;
    }

    private static void LoadNameConfiguration<TEntity, TMember>(this PropertyConfiguration @this, Expression<Func<TEntity, TMember>> expression)
    {
        var property = expression.GetPropertyInfo();
        var attribute = property.GetCustomAttribute<DisplayAttribute>();

        if (attribute != null)
        {
            @this.Name = attribute.Name;
        }
        else
        {
            throw new InvalidOperationException(string.Format("Property {0} on type {1} must be annotated with a {2}", property.Name, property.DeclaringType?.Name, nameof(DisplayAttribute)));
        }
    }

    private static void LoadRequiredConfiguration<TEntity, TMember>(this PrimitivePropertyConfiguration @this, Expression<Func<TEntity, TMember>> expression)
    {
        var property = expression.GetPropertyInfo();
        var attribute = property.GetCustomAttribute<RequiredAttribute>();

        if (attribute != null)
        {
            @this.IsRequired();
        }
    }

    internal static LengthPropertyConfiguration PropertyWithAnnotations<TEntity>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, string?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    internal static LengthPropertyConfiguration PropertyWithAnnotations<TEntity>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, byte[]?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    internal static PrimitivePropertyConfiguration PropertyWithAnnotations<TEntity>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, Stream?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    internal static DecimalPropertyConfiguration PropertyWithAnnotations<TEntity>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, decimal?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    internal static DecimalPropertyConfiguration PropertyWithAnnotations<TEntity, TMember>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, decimal>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, TimeOfDay?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, TimeOfDay>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, TimeOnly?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, TimeOnly>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, TimeSpan?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, TimeSpan>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, DateTimeOffset?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, DateTimeOffset>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    internal static UntypedPropertyConfiguration PropertyWithAnnotations<TEntity>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, object?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);

        return configuration;
    }

    internal static PrimitivePropertyConfiguration PropertyWithAnnotations<TEntity, TMember>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, TMember?>> propertyExpression)
        where TEntity : class
        where TMember : struct
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    internal static PrimitivePropertyConfiguration PropertyWithAnnotations<TEntity, TMember>(this EntityTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, TMember>> propertyExpression)
        where TEntity : class
        where TMember : struct
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }
}
