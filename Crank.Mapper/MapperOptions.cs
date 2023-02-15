using System;

namespace Crank.Mapper
{
    public interface IMapperOptions
    {
        public bool ThrowMappingNotFoundException { get; }
        public bool DisallowDuplicateMappingTypes { get; }
        public Action<Type, Type> MappingNotFoundEvent { get; }
        public bool IgnoreNullResultWhenCallingDestinationMap { get; }
    }

    public struct MapperOptions : IMapperOptions
    {
        public bool ThrowMappingNotFoundException { get; set; }
        public bool DisallowDuplicateMappingTypes { get; set; }
        public Action<Type, Type> MappingNotFoundEvent { get; set; }
        public bool IgnoreNullResultWhenCallingDestinationMap { get; set; }
    }
}
