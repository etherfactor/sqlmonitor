using AutoMapper;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IMapper"/>.
/// </summary>
internal static class IMapperExtensions
{
    /// <summary>
    /// Allows explicit mapping.
    /// </summary>
    /// <typeparam name="TFrom">The initial type.</typeparam>
    internal interface IMapExplicitly<TFrom>
    {
        /// <summary>
        /// Maps explicitly to a type.
        /// </summary>
        /// <typeparam name="TTo">The resulting type.</typeparam>
        /// <param name="membersToExpand">The names of properties to expand.</param>
        /// <returns>The mapped type.</returns>
        TTo To<TTo>(string[] membersToExpand);
    }

    private class ValueMapExplicitly<TFrom> : IMapExplicitly<TFrom>
    {
        private IMapper Mapper { get; }

        private TFrom Value { get; }

        internal ValueMapExplicitly(IMapper mapper, TFrom value)
        {
            Mapper = mapper;
            Value = value;
        }

        /// <inheritdoc/>
        public TTo To<TTo>(string[] membersToExpand)
        {
            IQueryable<TFrom> queryable = Value.Yield().AsQueryable();
            IQueryable<TTo> projected = Mapper.ProjectTo<TTo>(queryable, null, membersToExpand);
            var list = projected.ToList();

            return list.Single();
        }
    }

    /// <summary>
    /// Maps an object from one form to another, explicitly (will only expand chosen properties, not all properties).
    /// </summary>
    /// <typeparam name="TFrom">The initial type.</typeparam>
    /// <param name="this">Itself.</param>
    /// <param name="entity">The entity to map.</param>
    /// <returns>A wrapper to allow explicit mapping.</returns>
    internal static IMapExplicitly<TFrom> MapExplicitly<TFrom>(this IMapper @this, TFrom entity)
    {
        return new ValueMapExplicitly<TFrom>(@this, entity);
    }
}
