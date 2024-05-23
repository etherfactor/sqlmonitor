﻿using EtherGizmos.SqlMonitor.Api.Extensions.Dotnet;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace EtherGizmos.SqlMonitor.Configuration.Helpers;

internal static class ThrowHelper
{
    [DoesNotReturn]
    internal static void ForMissingConfiguration<TOptions, TProperty>(string rootPath, TOptions options, Expression<Func<TOptions, TProperty>> propertyExpression)
    {
        var propertyName = propertyExpression.GetPropertyInfo().Name;
        ForMissingConfiguration(rootPath, options, propertyName, typeof(TProperty));
    }

    [DoesNotReturn]
    internal static void ForMissingConfiguration<TOptions>(string rootPath, TOptions options, string propertyName, Type propertyType)
    {
        throw new InvalidOperationException($"The property at configuration path '{rootPath}:{propertyName}' of type '{propertyType}' must be specified.");
    }

    [DoesNotReturn]
    internal static void ForInvalidConfiguration<TOptions, TProperty>(string rootPath, TOptions options, Expression<Func<TOptions, TProperty>> propertyExpression, string intendedValue)
    {
        var propertyName = propertyExpression.GetPropertyInfo().Name;
        throw new InvalidOperationException($"The property at configuration path '{rootPath}:{propertyName}' of type '{typeof(TProperty)}' must {intendedValue}.");
    }
}