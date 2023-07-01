using AutoMapper;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

internal static class IMapperExtensions
{
    internal interface IMapExplicitly<TFrom>
    {
        TTo To<TTo>(string[] membersToExpand);
    }

    internal class ValueMapExplicitly<TFrom> : IMapExplicitly<TFrom>
    {
        private IMapper Mapper { get; }

        private TFrom Value { get; }

        public ValueMapExplicitly(IMapper mapper, TFrom value)
        {
            Mapper = mapper;
            Value = value;
        }

        public TTo To<TTo>(string[] membersToExpand)
        {
            IQueryable<TFrom> queryable = Value.Yield().AsQueryable();
            IQueryable<TTo> projected = Mapper.ProjectTo<TTo>(queryable, null, membersToExpand);
            var list = projected.ToList();

            return list.Single();
        }
    }

    internal static IMapExplicitly<TFrom> MapExplicitly<TFrom>(this IMapper @this, TFrom entity)
    {
        return new ValueMapExplicitly<TFrom>(@this, entity);
    }
}
