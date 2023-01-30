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

        public MapDestination<TDestination> MapFrom<TSource, TSource1>(TSource source, TSource1 source1, bool throwMapNotFoundException = true)
        {
            MapFrom(source, throwMapNotFoundException);
            MapFrom(source1, throwMapNotFoundException);
            return this;
        }

        public MapDestination<TDestination> MapFrom<TSource, TSource1, TSource2>(TSource source, TSource1 source1, TSource2 source2, bool throwMapNotFoundException = true)
        {
            MapFrom(source, source1, throwMapNotFoundException);
            MapFrom(source2, throwMapNotFoundException);
            return this;
        }
        public MapDestination<TDestination> MapFrom<TSource, TSource1, TSource2, TSource3>(TSource source, TSource1 source1, TSource2 source2, TSource3 source3, bool throwMapNotFoundException = true)
        {
            MapFrom(source, source1, source2, throwMapNotFoundException);
            MapFrom(source3, throwMapNotFoundException);
            return this;
        }

        public MapDestination<TDestination> Map(Action<TDestination> mapAction)
        {
            mapAction?.Invoke(Result);
            return this;
        }
    }
}
