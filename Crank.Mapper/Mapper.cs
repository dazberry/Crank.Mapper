using Crank.Mapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crank.Mapper
{
    public struct MapperOptions
    {
        public bool ThrowMappingNotFoundException { get; set; }
        public bool DisallowDuplicationMappingTypes { get; set; }
        public Action<Type, Type> MappingNotFoundEvent { get; set; }
        public Action<Exception> MappingFailedEvent { get; set; }
    }

    public class Mapper
    {

        private readonly MapperOptions _mapperOptions;

        private struct MappingInterfaceTypes
        {
            public IMapping Mapping { get; private set; }

            public Type MappingInterfaceType { get; private set; }

            public (Type source, Type destination) ArgumentTypes =>
                (MappingInterfaceType.GenericTypeArguments[0],
                 MappingInterfaceType.GenericTypeArguments[1]);

            public MappingInterfaceTypes(IMapping mapping)
            {
                Mapping = mapping;
                MappingInterfaceType = mapping
                    .GetType()
                    .GetInterfaces()
                    .Where(x => x.IsGenericType)
                    .FirstOrDefault();
            }

            public bool CompareArguments(Type source, Type destination)
            {
                var result = ArgumentTypes.source == source &&
                    ArgumentTypes.destination == destination;
                return result;
            }

            public bool CompareArguments(MappingInterfaceTypes compareTo)
            {
                var (source, destination) = compareTo.ArgumentTypes;
                return CompareArguments(source, destination);
            }
        }

        private readonly MappingInterfaceTypes[] _mappings;

        public Mapper(IEnumerable<IMapping> mappings, MapperOptions mapperOptions = default)
        {
            _mapperOptions = mapperOptions;

            _mappings = mappings
                .Select(mapping => new MappingInterfaceTypes(mapping))
                .ToArray();

            if (_mapperOptions.DisallowDuplicationMappingTypes)
            {
                var duplicates = _mappings.GroupBy(
                    x => x.ArgumentTypes,
                    y => y,
                    (x, y) =>
                    {
                        var matchingCount = y.Count(
                            z => z.CompareArguments(x.source, x.destination));
                        return (matchingCount > 1, x);
                    })
                    .Select(x => (dup: x.Item1, x.x))
                    .Where(x => x.dup);


                if (duplicates.Any())
                {
                    var (_, (source, destination)) = duplicates.First();
                    throw new DuplicateMappingException(source, destination);
                }
            }
        }

        private bool GetMapping<TSource, TDestination>(out IMapping<TSource, TDestination> mapping)
        {
            var mappingResult = false;
            try
            {
                var _mapping = _mappings.First(map =>
                    map.MappingInterfaceType == typeof(IMapping<TSource, TDestination>));
                mapping = _mapping.Mapping as IMapping<TSource, TDestination>;
                mappingResult = mapping != default;
            }
            catch (InvalidOperationException)
            {
                mapping = default;
            }
            finally
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[Mapper] {(mappingResult ? "Success" : "Failed")}: TryGetMapping<{typeof(TSource)}, {typeof(TDestination)}>");
#endif
                if (!mappingResult)
                    _mapperOptions.MappingNotFoundEvent?.Invoke(typeof(TSource), typeof(TDestination));
            }
            return mappingResult;
        }

        public bool TryGetMapping<TSource, TDestination>(out IMapping<TSource, TDestination> mapping)
        {
            var result = GetMapping(out mapping);
            if (!result && _mapperOptions.ThrowMappingNotFoundException)
                throw new MappingNotFoundException<TSource, TDestination>();
            return result;
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination = default) =>
            TryGetMapping<TSource, TDestination>(out var mapping)
                ? mapping.Map(source, destination)
                : destination;

        public TDestination Map<TSource, TDestination>(TSource source, Action<TDestination> destinationAction)
        {
            if (TryGetMapping<TSource, TDestination>(out var mapping))
            {
                var destination = mapping.Map(source);
                destinationAction?.Invoke(destination);
                return destination;
            }
            return default;
        }

        public bool TryMap<TSource, TDestination>(TSource source, out TDestination destination)
        {
            var haveMapping = GetMapping<TSource, TDestination>(out var mapping);
            destination = haveMapping ? mapping.Map(source) : default;
            if (haveMapping)
            {
                try
                {
                    destination = mapping.Map(source);
                    return true;
                }
                catch (Exception ex)
                {
                    _mapperOptions.MappingFailedEvent?.Invoke(ex);
                }
            }
            return false;
        }

        public MapDestination<TDestination> MapTo<TDestination>()
            where TDestination : class, new()
        {
            var mapping = new MapDestination<TDestination>(this, new TDestination());
            return mapping;
        }

        public MapDestination<TDestination> MapTo<TDestination>(TDestination destination)
            where TDestination : class
        {
            var mapping = new MapDestination<TDestination>(this, destination);
            return mapping;
        }

    }
}
