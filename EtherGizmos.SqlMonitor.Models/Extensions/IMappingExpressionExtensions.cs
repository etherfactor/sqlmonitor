using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Api.Abstractions;
using EtherGizmos.SqlMonitor.Models.Database.Abstractions;

namespace EtherGizmos.SqlMonitor.Models.Extensions;

internal static class IMappingExpressionExtensions
{
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
