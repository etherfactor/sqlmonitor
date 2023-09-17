using EtherGizmos.SqlMonitor.Models.Api.Abstractions;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Models.Extensions;

/// <summary>
/// Provides extension methods for <see cref="StructuralTypeConfiguration{TEntityType}"/>.
/// </summary>
internal static class StructuralTypeConfigurationExtensions
{
    /// <summary>
    /// Adds audit properties to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    internal static void AuditPropertiesWithAnnotations<TEntity>(this StructuralTypeConfiguration<TEntity> @this)
        where TEntity : class, IAuditableDTO
    {
        @this.PropertyWithAnnotations(e => e.CreatedAt);
        @this.PropertyWithAnnotations(e => e.CreatedByUserId);
        @this.PropertyWithAnnotations(e => e.ModifiedAt);
        @this.PropertyWithAnnotations(e => e.ModifiedByUserId);
    }

    /// <summary>
    /// Adds a collection property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TMember">The type of collection entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="collectionPropertyExpression">The collection property selector.</param>
    /// <returns>The property builder.</returns>
    internal static CollectionPropertyConfiguration CollectionPropertyWithAnnotations<TEntity, TMember>(this StructuralTypeConfiguration<TEntity> @this, Expression<Func<TEntity, IEnumerable<TMember>>> collectionPropertyExpression)
        where TEntity : class
    {
        var configuration = @this.CollectionProperty(collectionPropertyExpression);
        configuration.LoadNameConfiguration(collectionPropertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a complex property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TMember">The type of complex entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="complexPropertyExpression">The complex property selector.</param>
    /// <returns>The property builder.</returns>
    internal static ComplexPropertyConfiguration ComplexPropertyWithAnnotations<TEntity, TMember>(this StructuralTypeConfiguration<TEntity> @this, Expression<Func<TEntity, TMember>> complexPropertyExpression)
        where TEntity : class
    {
        var configuration = @this.ComplexProperty(complexPropertyExpression);
        configuration.LoadNameConfiguration(complexPropertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a navigational set to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TMember">The type of navigational entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="navigationPropertyExpression">The navigational selector.</param>
    /// <returns>The property builder.</returns>
    internal static NavigationPropertyConfiguration HasManyWithAnnotations<TEntity, TMember>(this StructuralTypeConfiguration<TEntity> @this, Expression<Func<TEntity, IEnumerable<TMember>>> navigationPropertyExpression)
        where TEntity : class
        where TMember : class
    {
        var configuration = @this.HasMany(navigationPropertyExpression);
        configuration.LoadNameConfiguration(navigationPropertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds an optional single to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TMember">The type of navigational entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="navigationPropertyExpression">The navigational selector.</param>
    /// <returns>The property builder.</returns>
    internal static NavigationPropertyConfiguration HasOptionalWithAnnotations<TEntity, TMember>(this StructuralTypeConfiguration<TEntity> @this, Expression<Func<TEntity, TMember>> navigationPropertyExpression)
        where TEntity : class
        where TMember : class
    {
        var configuration = @this.HasOptional(navigationPropertyExpression);
        configuration.LoadNameConfiguration(navigationPropertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a required single to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TMember">The type of navigational entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="navigationPropertyExpression">The navigational selector.</param>
    /// <returns>The property builder.</returns>
    internal static NavigationPropertyConfiguration HasRequiredWithAnnotations<TEntity, TMember>(this StructuralTypeConfiguration<TEntity> @this, Expression<Func<TEntity, TMember?>> navigationPropertyExpression)
        where TEntity : class
        where TMember : class
    {
        var configuration = @this.HasRequired(navigationPropertyExpression);
        configuration.LoadNameConfiguration(navigationPropertyExpression);

        return configuration;
    }

    /// <summary>
    /// Loads a property name from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TMember">The type of entity member.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="expression">The property selector.</param>
    /// <exception cref="InvalidOperationException"></exception>
    private static void LoadNameConfiguration<TEntity, TMember>(this PropertyConfiguration @this, Expression<Func<TEntity, TMember>> expression)
    {
        //Load the DisplayAttribute
        var property = expression.GetPropertyInfo();
        var attribute = property.GetCustomAttribute<DisplayAttribute>();

        //Fetch the name and ensure it exists
        string name = attribute?.Name
            ?? throw new InvalidOperationException(string.Format("Property '{0}' on type '{1}' must be annotated with a '{2}' and specify the '{3}' property",
                property.Name, property.DeclaringType?.Name, nameof(DisplayAttribute), nameof(attribute.Name)));

        //Set the property display name
        @this.Name = name;
    }

    /// <summary>
    /// Loads a required status from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TMember">The type of entity member.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="expression">The property selector.</param>
    private static void LoadRequiredConfiguration<TEntity, TMember>(this PrimitivePropertyConfiguration @this, Expression<Func<TEntity, TMember>> expression)
    {
        //Load the RequiredAttribute, and if it exists, mark the property as such
        var property = expression.GetPropertyInfo();
        var attribute = property.GetCustomAttribute<RequiredAttribute>();

        if (attribute != null)
        {
            @this.IsRequired();
        }
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static LengthPropertyConfiguration PropertyWithAnnotations<TEntity>(this StructuralTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, string?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static LengthPropertyConfiguration PropertyWithAnnotations<TEntity>(this StructuralTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, byte[]?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static PrimitivePropertyConfiguration PropertyWithAnnotations<TEntity>(this StructuralTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, Stream?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static DecimalPropertyConfiguration PropertyWithAnnotations<TEntity>(this StructuralTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, decimal?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static DecimalPropertyConfiguration PropertyWithAnnotations<TEntity, TMember>(this StructuralTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, decimal>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this StructuralTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, TimeOfDay?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this StructuralTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, TimeOfDay>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this StructuralTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, TimeOnly?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this StructuralTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, TimeOnly>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this StructuralTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, TimeSpan?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this StructuralTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, TimeSpan>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this StructuralTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, DateTimeOffset?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static PrecisionPropertyConfiguration PropertyWithAnnotations<TEntity>(this StructuralTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, DateTimeOffset>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static UntypedPropertyConfiguration PropertyWithAnnotations<TEntity>(this StructuralTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, object?>> propertyExpression)
        where TEntity : class
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static PrimitivePropertyConfiguration PropertyWithAnnotations<TEntity, TMember>(this StructuralTypeConfiguration<TEntity> @this,
        Expression<Func<TEntity, TMember?>> propertyExpression)
        where TEntity : class
        where TMember : struct
    {
        var configuration = @this.Property(propertyExpression);
        configuration.LoadNameConfiguration(propertyExpression);
        configuration.LoadRequiredConfiguration(propertyExpression);

        return configuration;
    }

    /// <summary>
    /// Adds a property to an entity, reading from annotations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="propertyExpression">The property selector.</param>
    /// <returns>The property builder.</returns>
    internal static PrimitivePropertyConfiguration PropertyWithAnnotations<TEntity, TMember>(this StructuralTypeConfiguration<TEntity> @this,
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
