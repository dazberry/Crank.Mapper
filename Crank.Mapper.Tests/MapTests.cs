using Crank.Mapper.Interfaces;
using Crank.Mapper.Tests.Mappings;
using Crank.Mapper.Tests.Models;
using System;
using Xunit;

namespace Crank.Mapper.Tests
{
    public class MapTests
    {
        public MapTests()
        {
            _mapper = new Mapper(
                new[] { new MapUserModel_To_UserEntity() });
        }

        private readonly Mapper _mapper;

        private static UserModel CreateUserModel() =>
            new UserModel()
            {
                UserId = Guid.NewGuid(),
                Username = "username123",
                UserAccess = UserAccessModel.Basic,
            };

        [Fact]
        public void WhenMappingEntities_IfAMappingExists_ShouldReturnDestinationEntity()
        {
            //given
            var userModel = CreateUserModel();

            //when
            var userEntity = _mapper.Map<UserModel, UserEntity>(userModel);

            //then
            Assert.NotNull(userEntity);
            Assert.Equal($"{userModel.UserId:n}", userEntity.RowKey);
            Assert.Equal(userModel.Username, userEntity.Username);
            Assert.Equal((int)userModel.UserAccess, userEntity.UserAccess);
        }

        [Fact]
        public void WhenMappingEntities_IfAMappingDoesNotExist_ReturnANullEntity()
        {
            //given
            UserEntity userEntity = new UserEntity();

            //when the mapping does not exist
            var userModel = _mapper.Map<UserEntity, UserModel>(userEntity);

            //then
            Assert.Null(userModel);
        }

        [Fact]
        public void WhenMappingEntities_IfAMappingDoesNotExist_ShowThrowMappingNotFoundException()
        {
            // given a mapper with the ThrowMappingNotFound flag set
            var mapper = new Mapper(
                new[] { new MapUserModel_To_UserEntity() },
                new MapperOptions() { ThrowMappingNotFoundException = true }
            );
            var userEntity = new UserEntity();

            //when the mapping does not exist
            Assert.Throws<MappingNotFoundException<UserEntity, UserModel>>(() =>
            {
                var userModel = mapper.Map<UserEntity, UserModel>(userEntity);
            });
        }

    }
}
