using Crank.Mapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Crank.Initialise
{
    public static class CrankInitialisationHelper
    {
        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        public static IEnumerable<Type> GetIMappingTypesFromAssembly(Assembly assembly) =>
            GetLoadableTypes(assembly)
                .Where(typeof(IMapping).IsAssignableFrom)
                .Where(x => x.IsClass);
    }
}
