using Crank.Mapper.Interfaces;

namespace Crank.Mapper.Microsoft.DependingInjection.Tests
{
    public class TryMapAStringToAnInt : IMapping<string, int>
    {
        public int Map(string source, int destination = 0)
        {
            if (int.TryParse(source, out int result))
                destination = result;

            return destination;
        }
    }
}
