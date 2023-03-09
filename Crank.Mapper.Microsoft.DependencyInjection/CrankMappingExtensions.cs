using Crank.Initialise;
using Crank.Mapper.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Crank.Mapper.Microsoft.DependencyInjection
{
    public static class CrankMappingExtensions
    {
        public static IServiceCollection RegisterMapper(this IServiceCollection services, MapperOptions mapperOptions = default)
        {
            services.AddSingleton(
                srv => new Mapper(
                    srv.GetServices<IMapping>(), mapperOptions));

            return services;
        }

        public static IServiceCollection RegisterMappings(this IServiceCollection services, IEnumerable<Assembly> assemblies = null)
        {
            assemblies ??= AppDomain.CurrentDomain.GetAssemblies();

            var mappings = assemblies.SelectMany(assm =>
            {
                return CrankInitialisationHelper.GetIMappingTypesFromAssembly(assm);
            });

            foreach (var mapping in mappings)
            {
                services.AddSingleton(typeof(IMapping), mapping);
                services.AddSingleton(mapping);
            }
            return services;
        }

        public static IServiceCollection RegisterMappings(this IServiceCollection services, Func<IEnumerable<Assembly>, IEnumerable<Assembly>> filterAssemblies)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().AsEnumerable();
            if (filterAssemblies != null)
                assemblies = filterAssemblies.Invoke(assemblies);

            return RegisterMappings(services, assemblies.ToArray());
        }
    }
}
