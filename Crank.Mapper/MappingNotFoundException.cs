using System;

namespace Crank.Mapper
{
    public class MappingNotFoundException : Exception
    {
        public MappingNotFoundException(Type TSource, Type TDestination)
            : base($"No mapping registered from source: {TSource} to destination: {TDestination}")
        {
        }
    }

    public class MappingNotFoundException<TSource, TDestination> : MappingNotFoundException
    {
        public MappingNotFoundException() : base(typeof(TSource), typeof(TDestination))
        {
        }
    }

}
