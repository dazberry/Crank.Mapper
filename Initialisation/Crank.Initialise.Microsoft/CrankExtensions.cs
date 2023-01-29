using Crank.Mapper;
using Crank.Mapper.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace Crank.Initialise.Microsoft
{
    public static class CrankExtensions
    {
        public static void RegisterMapper(this IServiceCollection services, MapperOptions mapperOptions = default)
        {
            services.AddSingleton(
                srv => new Mapper.Mapper(
                    srv.GetServices<IMapping>(), mapperOptions));
        }

        public static void RegisterMappings(this IServiceCollection services, Assembly[] assemblies)
        {
            var mappings = assemblies.SelectMany(assm =>
            {
                return CrankInitialisationHelper.GetTypesFromAssembly(assm);
            });

            foreach (var mapping in mappings)
            {
                services.AddSingleton(typeof(IMapping), mapping);
                services.AddSingleton(mapping);
            }
        }
    }
}
