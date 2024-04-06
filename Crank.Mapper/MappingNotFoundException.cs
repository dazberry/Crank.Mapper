using System;

namespace Crank.Mapper
{
    public class MappingNotFoundException : Exception
    {
        public MappingNotFoundException(Type TSource, Type TDestination)
            : base($"No mapping registered from source: {TSource} to destination: {TDestination}")
        {
        }

        public MappingNotFoundException(Type TSource, Type TSource2, Type TDestination)
            : base($"No mapping registered from both source: {TSource} and {TSource2} to destination: {TDestination}")
        {
        }
    }

    public class MappingNotFoundException<TSource, TDestination> : MappingNotFoundException
    {
        public MappingNotFoundException() : base(typeof(TSource), typeof(TDestination))
        {
        }
    }

    public class MappingNotFoundException<TSource, TSource2, TDestination> : MappingNotFoundException
    {
        public MappingNotFoundException() : base(typeof(TSource), typeof(TSource2), typeof(TDestination))
        {
        }
    }

}
