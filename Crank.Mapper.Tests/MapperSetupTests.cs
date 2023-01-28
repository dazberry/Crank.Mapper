using Crank.Mapper.Interfaces;
using Crank.Mapper.Tests.Mappings;
using Xunit;

namespace Crank.Mapper.Tests
{

    public class MapperSetupTests
    {
        [Fact]
        public void WithDuplicateMappingsTypes_CallingTheMapper_ShouldCallTheFirstMappingOnly()
        {
            //given
            var intToStringMap = new MapInt_To_String();
            var intToStringExMap = new MapInt_To_StringThrowException();
            var mappings = new IMapping[] { intToStringMap, intToStringExMap };
            var mapper = new Mapper(mappings);

            //when
            var result = mapper.Map<int, string>(123);

            //then
            Assert.Equal("123", result);
        }

        [Fact]
        public void WithDuplicateMappingsTypes_CallingTheMapper_ShouldCallTheFirstMappingOnly2()
        {
            //given
            var intToStringMap = new MapInt_To_String();
            var intToStringExMap = new MapInt_To_StringThrowException();
            var mappings = new IMapping[] { intToStringExMap, intToStringMap };
            var mapper = new Mapper(mappings);

            //when
            Assert.Throws<MapInt_To_StringThrowException.MapInt_To_StringException>(() =>
            {
                mapper.Map<int, string>(123);
            });
        }

        [Fact]
        public void WithDuplicationMappings_AndDisallowDuplicationMappingTypesIsSet_ShouldThrowAnException()
        {
            //given
            var intToStringMap = new MapInt_To_String();
            var intToStringExMap = new MapInt_To_StringThrowException();
            var mapUserModelToEntity = new MapUserModel_To_UserEntity();
            var mappings = new IMapping[] { mapUserModelToEntity, intToStringExMap, intToStringMap };
            var options = new MapperOptions() { DisallowDuplicationMappingTypes = true };

            //when/then
            var exception = Assert.Throws<DuplicateMappingException>(() =>
            {
                var mapper = new Mapper(mappings, options);
            });

            Assert.StartsWith("Two or more mappings have been registered with the same types.", exception.Message);
        }
    }
}
