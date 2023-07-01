using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Api.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;

namespace EtherGizmos.SqlMonitor.Models.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IMappingExpression{TSource, TDestination}"/>.
/// </summary>
internal static class IMappingExpressionExtensions
{
    /// <summary>
    /// Ignore for now, affected by https://github.com/OData/AspNetCoreOData/issues/887.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <param name="this"></param>
    /// <returns></returns>
    internal static IMappingExpression<TSource, TDestination> MapAuditingColumnsToDTO<TSource, TDestination>(this IMappingExpression<TSource, TDestination> @this)
        where TSource : IAuditable
        where TDestination : IAuditableDTO
    {
        @this.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        @this.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        @this.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        @this.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);

        return @this;
    }

    /// <summary>
    /// Ignore for now, affected by https://github.com/OData/AspNetCoreOData/issues/887.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <param name="this"></param>
    /// <returns></returns>
    internal static IMappingExpression<TSource, TDestination> MapAuditingColumnsFromDTO<TSource, TDestination>(this IMappingExpression<TSource, TDestination> @this)
        where TSource : IAuditableDTO
        where TDestination : IAuditable
    {
        @this.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        @this.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        @this.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        @this.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);

        return @this;
    }
}
