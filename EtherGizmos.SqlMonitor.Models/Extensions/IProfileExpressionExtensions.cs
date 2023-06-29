using AutoMapper;
using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EtherGizmos.SqlMonitor.Models.Extensions;

internal static class IProfileExpressionExtensions
{
    internal static IMappingExpression<TSource, TDestination> IgnoreAllMembers<TSource, TDestination>(this IMappingExpression<TSource, TDestination> @this)
    {
        var destinationType = typeof(TDestination);

        foreach (var property in destinationType.GetProperties())
            @this.ForMember(property.Name, opt => opt.Ignore());

        return @this;
    }

    internal static IMappingExpression<TSource, TDestination> MapMember<TSource, TSourceMember, TDestination, TDestinationMember>(
        this IMappingExpression<TSource, TDestination> @this, Expression<Func<TDestination, TDestinationMember>> destinationMember,
        Expression<Func<TSource, TSourceMember>> sourceMember, Action<IMemberConfigurationExpression<TSource, TDestination, TDestinationMember>>? memberOptions = null)
    {
        @this.ForMember(destinationMember, configure =>
        {
            if (memberOptions != null)
                memberOptions(configure);

            configure.MapFrom(sourceMember);
        });

        return @this;
    }

    internal static IMappingExpression<TSource, TDestination> MapPath<TSource, TSourceMember, TDestination, TDestinationMember>(
        this IMappingExpression<TSource, TDestination> @this, Expression<Func<TDestination, TDestinationMember>> destinationPath,
        Expression<Func<TSource, TSourceMember>> sourcePath, Action<IPathConfigurationExpression<TSource, TDestination, TDestinationMember>>? pathOptions = null)
    {
        @this.ForPath(destinationPath, configure =>
        {
            if (pathOptions != null)
                pathOptions(configure);

            configure.MapFrom(sourcePath);
        });

        return @this;
    }
}
