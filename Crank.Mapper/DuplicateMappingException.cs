using System;

namespace Crank.Mapper
{
    public class DuplicateMappingException : Exception
    {
        private const string _errorMessage = "Two or more mappings have been registered with the same types.";

        public DuplicateMappingException() : base(_errorMessage)
        {
        }

        public DuplicateMappingException(Type source, Type destination)
            : base($"{_errorMessage} IMapping<{source}, {destination}>")
        {
        }
    }

    public class DuplicateMappingException<TSource, TDestination> : DuplicateMappingException
    {
        public DuplicateMappingException() : base(typeof(TSource), typeof(TDestination))
        {
        }
    }
}
