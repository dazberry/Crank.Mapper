﻿using Crank.Mapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Crank.Mapper
{

    public class Mapper
    {
        private readonly MapperOptions _mapperOptions;
        public IMapperOptions Options => _mapperOptions;

        private struct MappingInterfaceTypes
        {
            public IMapping Mapping { get; private set; }

            public Type MappingInterfaceType { get; private set; }

            public (Type source, Type destination) GetGenericTypeArguments() =>
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
        }

        private readonly MappingInterfaceTypes[] _mappings;

        public Mapper(IEnumerable<IMapping> mappings, MapperOptions mapperOptions = default)
        {
            _mapperOptions = mapperOptions;

            _mappings = mappings
                .Select(mapping => new MappingInterfaceTypes(mapping))
                .ToArray();

            if (_mapperOptions.DisallowDuplicateMappingTypes)
            {
                var duplicates = GetDuplicateMappings();
                if (duplicates.Any())
                {
                    var (source, destination) = duplicates.First();
                    throw new DuplicateMappingException(source, destination);
                }
            }
        }

        private IEnumerable<(Type source, Type destination)> GetDuplicateMappings()
        {
            var duplicates = _mappings.GroupBy(
                   x => x.GetGenericTypeArguments(),
                   y => y,
                   (x, y) =>
                   {
                       var count = y.Count(z => z.GetGenericTypeArguments() == x);
                       return (isDuplicate: count > 1, x.source, x.destination);
                   })
                   .Where(x => x.isDuplicate)
                   .Select(x => (x.source, x.destination));
            return duplicates;
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

        private bool GetMapping<TSource, TSource2, TDestination>(out IMapping<TSource, TSource2, TDestination> mapping)
        {
            var mappingResult = false;
            try
            {
                var _mapping = _mappings.First(map =>
                    map.MappingInterfaceType == typeof(IMapping<TSource, TSource2, TDestination>));
                mapping = _mapping.Mapping as IMapping<TSource, TSource2, TDestination>;
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

        public bool TryGetMapping<TSource, TSource2, TDestination>(out IMapping<TSource, TSource2, TDestination> mapping)
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

        public TDestination Map<TSource, TSource2, TDestination>(TSource source, TSource2 source2, TDestination destination = default) =>
            TryGetMapping<TSource, TSource2, TDestination>(out var mapping)
                ? mapping.Map(source, source2, destination)
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

        public TDestination Map<TSource, TSource2, TDestination>(TSource source, TSource2 source2, Action<TDestination> destinationAction)
        {
            if (TryGetMapping<TSource, TSource2, TDestination>(out var mapping))
            {
                var destination = mapping.Map(source, source2);
                destinationAction?.Invoke(destination);
                return destination;
            }
            return default;
        }

        public bool TryMap<TSource, TDestination>(TSource source, out TDestination destination, Action<Exception> MappingFailedEvent = default)
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
                    MappingFailedEvent?.Invoke(ex);
                }
            }
            return false;
        }

        public bool TryMap<TSource, TSource2, TDestination>(TSource source, TSource2 source2, out TDestination destination, Action<Exception> MappingFailedEvent = default)
        {
            var haveMapping = GetMapping<TSource, TSource2, TDestination>(out var mapping);
            destination = haveMapping ? mapping.Map(source, source2) : default;
            if (haveMapping)
            {
                try
                {
                    destination = mapping.Map(source, source2);
                    return true;
                }
                catch (Exception ex)
                {
                    MappingFailedEvent?.Invoke(ex);
                }
            }
            return false;
        }

        public MapDestination<TDestination> MapTo<TDestination>()
        {
            var mapping = new MapDestination<TDestination>(this);
            return mapping;
        }

        public MapDestination<TDestination> MapTo<TDestination>(TDestination destination)
        {
            var mapping = new MapDestination<TDestination>(this, destination);
            return mapping;
        }

        public MapDestination<TDestination> MapNew<TDestination>()
            where TDestination : class, new()
        {
            var mapping = new MapDestination<TDestination>(this, new TDestination());
            return mapping;
        }

    }
}
