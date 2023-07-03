using EtherGizmos.SqlMonitor.Models.Api.Abstractions;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IEdmModel"/>.
/// </summary>
internal static class IEdmModelExtensions
{
    /// <summary>
    /// Intended to work around https://github.com/OData/AspNetCoreOData/issues/887, but has some caveats with interfaces.
    /// Ignore for the time being.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="this"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal static IEdmModel ApplyClrAnnotationFixForAudit<TEntity>(this IEdmModel @this)
        where TEntity : IAuditableDTO
    {
        var edmType = (IEdmStructuredType?)@this.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(e => e.Name == typeof(TEntity).GetCustomAttribute<DisplayAttribute>()?.Name)
            ?? @this.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault(e => e.Name == typeof(TEntity).GetCustomAttribute<DisplayAttribute>()?.Name)
            ?? throw new InvalidOperationException("Unexpected configuration");

        ApplyClrAnnotationFix<TEntity>(@this, edmType, nameof(IAuditableDTO.CreatedAt));
        ApplyClrAnnotationFix<TEntity>(@this, edmType, nameof(IAuditableDTO.CreatedByUserId));
        ApplyClrAnnotationFix<TEntity>(@this, edmType, nameof(IAuditableDTO.ModifiedAt));
        ApplyClrAnnotationFix<TEntity>(@this, edmType, nameof(IAuditableDTO.ModifiedByUserId));

        return @this;
    }

    /// <summary>
    /// Intended to work around https://github.com/OData/AspNetCoreOData/issues/887, but has some caveats with interfaces.
    /// Ignore for the time being.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="model"></param>
    /// <param name="edmType"></param>
    /// <param name="propertyName"></param>
    private static void ApplyClrAnnotationFix<TEntity>(IEdmModel model, IEdmStructuredType edmType, string propertyName)
    {
        var property = typeof(TEntity).GetProperty(propertyName)!;
        string? displayName = property.GetCustomAttribute<DisplayAttribute>()?.Name;

        var edmProperty = edmType.FindProperty(displayName ?? propertyName);
        model.SetAnnotationValue(edmProperty, new ClrPropertyInfoAnnotation(property));
    }
}
