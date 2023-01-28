using Crank.Mapper.Interfaces;
using Crank.Mapper.Tests.Mappings;
using Crank.Mapper.Tests.Models;
using System;
using Xunit;

namespace Crank.Mapper.Tests
{
    public class MapTests
    {
        [Fact]
        public void WhenMappingEntities_IfAMappingExists_ShouldReturnDestinationEntity()
        {
            var mapper = new Mapper(
                new[] { new MapUserModel_To_UserEntity() });

            var userModel = new UserModel()
            {
                UserId = Guid.NewGuid(),
                Username = "username123"
            };

            var userEntity = mapper.Map<UserModel, UserEntity>(userModel);

            Assert.NotNull(userEntity);
        }

        [Fact]
        public void WhenMappingEntities_IfAMappingDoesNotExist_ReturnANullEntity()
        {
            var mapper = new Mapper(Array.Empty<IMapping>());

            var userModel = new UserModel()
            {
                UserId = Guid.NewGuid(),
                Username = "username123"
            };

            var userEntity = mapper.Map<UserModel, UserEntity>(userModel);

            Assert.Null(userEntity);
        }

        [Fact]
        public void WhenMappingEntities_IfAMappingDoesNotExist_ShowThrowMappingNotFoundException()
        {
            var mapper = new Mapper(
                Array.Empty<IMapping>(),
                new MapperOptions() { ThrowMappingNotFoundException = true }
            );

            var userModel = new UserModel()
            {
                UserId = Guid.NewGuid(),
                Username = "username123"
            };


            UserEntity userEntity = default;
            Assert.Throws<MappingNotFoundException<UserModel, UserEntity>>(() =>
            {
                userEntity = mapper.Map<UserModel, UserEntity>(userModel);
            });

            Assert.Null(userEntity);
        }

    }
}
