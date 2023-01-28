using Crank.Mapper.Interfaces;
using Crank.Mapper.Tests.Mappings;
using Crank.Mapper.Tests.Models;
using System;
using Xunit;

namespace Crank.Mapper.Tests
{
    public class MapToTests
    {
        [Fact]
        public void WhenMappingToEntities_IfAMappingExists_Success()
        {
            var mapper = new Mapper(
                new[] { new MapUserModel_To_UserEntity() });

            var userModel = new UserModel()
            {
                UserId = Guid.NewGuid(),
                Username = "username123"
            };

            var userEntity = mapper.MapTo<UserEntity>()
                .MapFrom<UserModel>(userModel)
                .Result;

            Assert.NotNull(userEntity);
        }

        [Fact]
        public void WhenMappingToEntities_IfAMappingDoesNotExists_ThrowANotFoundException()
        {
            var mapper = new Mapper(Array.Empty<IMapping>());

            var userModel = new UserModel()
            {
                UserId = Guid.NewGuid(),
                Username = "username123"
            };

            UserEntity userEntity = default;
            Assert.Throws<MappingNotFoundException<UserModel, UserEntity>>(() =>
            {
                userEntity = mapper.MapTo<UserEntity>()
                    .MapFrom<UserModel>(userModel)
                    .Result;
            });

            Assert.Null(userEntity);
        }
    }
}
