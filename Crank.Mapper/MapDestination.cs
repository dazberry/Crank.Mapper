using System;

namespace Crank.Mapper
{
    public struct MapDestination<TDestination>
    {
        private readonly Mapper _mapper;
        public TDestination Result { get; private set; }

        public MapDestination(Mapper mapper, TDestination destination = default)
        {
            _mapper = mapper;
            Result = destination;
        }

        public MapDestination<TDestination> MapFrom<TSource>(TSource source, bool throwMapNotFoundException = true)
        {
            if (!_mapper.TryGetMapping<TSource, TDestination>(out var mapping))
            {
                if (throwMapNotFoundException)
                    throw new MappingNotFoundException<TSource, TDestination>();
                return this;
            }

            Result = mapping.Map(source, Result);
            return this;
        }

        public MapDestination<TDestination> MapFromBoth<TSource, TSource2>(TSource source, TSource2 source2, bool throwMapNotFoundException = true)
        {
            if (!_mapper.TryGetMapping<TSource, TSource2, TDestination>(out var mapping))
            {
                if (throwMapNotFoundException)
                    throw new MappingNotFoundException<TSource, TSource2, TDestination>();
                return this;
            }
            Result = mapping.Map(source, source2, Result);
            return this;
        }

        public MapDestination<TDestination> MapFrom<TSource, TSource2>(TSource source, TSource2 source2 , bool throwMapNotFoundException = true)
        {
            MapFrom(source, throwMapNotFoundException);
            MapFrom(source2, throwMapNotFoundException);
            return this;
        }

        public MapDestination<TDestination> MapFrom<TSource, TSource2, TSource3>(TSource source, TSource2 source2, TSource3 source3, bool throwMapNotFoundException = true)
        {
            MapFrom(source, source2, throwMapNotFoundException);
            MapFrom(source3, throwMapNotFoundException);
            return this;
        }
        public MapDestination<TDestination> MapFrom<TSource, TSource2, TSource3, TSource4>(TSource source, TSource2 source2, TSource3 source3, TSource4 source4, bool throwMapNotFoundException = true)
        {
            MapFrom(source, source2, source3, throwMapNotFoundException);
            MapFrom(source4, throwMapNotFoundException);
            return this;
        }

        public MapDestination<TDestination> Map(Action<TDestination> mapAction)
        {
            if ((Result == null) && !_mapper.Options.IgnoreNullResultWhenCallingDestinationMap)
                throw new MapDestinationNullResultException();

            mapAction?.Invoke(Result);
            return this;
        }
    }
}
