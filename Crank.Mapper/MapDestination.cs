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
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
        }

        public MapDestination<TDestination> MapFrom<TSource>(TSource source)
        {
            if (!_mapper.TryGetMapping<TSource, TDestination>(out var mapping))
                throw new MappingNotFoundException<TSource, TDestination>();

            Result = mapping.Map(source, Result);
            return this;
        }

        public MapDestination<TDestination> MapFrom<TSource, TSource1>(TSource source, TSource1 source1)
        {
            MapFrom(source);
            MapFrom(source1);
            return this;
        }

        public MapDestination<TDestination> MapFrom<TSource, TSource1, TSource2>(TSource source, TSource1 source1, TSource2 source2)
        {
            MapFrom(source, source1);
            MapFrom(source2);
            return this;
        }
        public MapDestination<TDestination> MapFrom<TSource, TSource1, TSource2, TSource3>(TSource source, TSource1 source1, TSource2 source2, TSource3 source3)
        {
            MapFrom(source, source1, source2);
            MapFrom(source3);
            return this;
        }

        public MapDestination<TDestination> Map(Action<TDestination> mapAction)
        {
            mapAction?.Invoke(Result);
            return this;
        }
    }
}
