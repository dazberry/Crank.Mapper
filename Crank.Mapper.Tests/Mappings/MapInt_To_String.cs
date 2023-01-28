using Crank.Mapper.Interfaces;
using System;

namespace Crank.Mapper.Tests.Mappings
{
    public class MapInt_To_String : IMapping<int, string>
    {
        public string Map(int source, string destination = null)
        {
            destination = $"{source}";
            return destination;
        }
    }

    public class MapInt_To_StringThrowException : IMapping<int, string>
    {

        public class MapInt_To_StringException : Exception
        {
        }

        public string Map(int source, string destination = null)
        {
            throw new MapInt_To_StringException();
        }
    }
}
