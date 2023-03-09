using Crank.Mapper.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;

namespace Crank.Mapper.Microsoft.DependingInjection.Tests
{
    public class RegistrationTests
    {
        public static ServiceProvider CreateServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                 .RegisterMappings()
                 .RegisterMapper();
            return serviceCollection.BuildServiceProvider();
        }

        public static ServiceProvider CreateServiceProviderWithFilter()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                 .RegisterMappings(asms => asms.Where(asm => asm.FullName.StartsWith("Crank.Mapper")))
                 .RegisterMapper();
            return serviceCollection.BuildServiceProvider();
        }

        public static ServiceProvider CreateServiceProviderWithAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
               .Where(assm => assm.FullName.StartsWith("Crank.Mapper"));

            var serviceCollection = new ServiceCollection();
            serviceCollection
                 .RegisterMappings(assemblies)
                 .RegisterMapper();
            return serviceCollection.BuildServiceProvider();
        }


        [Fact]
        public void RegisterMapperAndAssociatedTests_AllAssemblies()
        {
            //given
            var serviceProvider = CreateServiceProvider();
            var mapper = serviceProvider.GetService<Mapper>();
            var stringValue = "123";

            //when
            var result = mapper.Map<string, int>(stringValue);

            //then
            Assert.Equal(123, result);
        }

        [Fact]
        public void RegisterMapperAndAssociatedTests_SuppliedAssemblies()
        {
            //given
            var serviceProvider = CreateServiceProviderWithAssemblies();
            var mapper = serviceProvider.GetService<Mapper>();
            var stringValue = "123";

            //when
            var result = mapper.Map<string, int>(stringValue);

            //then
            Assert.Equal(123, result);
        }

        [Fact]
        public void RegisterMapperAndAssociatedTests_FilteredAssemblies()
        {
            //given
            var serviceProvider = CreateServiceProviderWithFilter();
            var mapper = serviceProvider.GetService<Mapper>();
            var stringValue = "123";

            //when
            var result = mapper.Map<string, int>(stringValue);

            //then
            Assert.Equal(123, result);
        }
    }
}
