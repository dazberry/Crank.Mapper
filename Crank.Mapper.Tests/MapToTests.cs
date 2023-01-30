using Crank.Mapper.Tests.Mappings;
using Crank.Mapper.Tests.Models;
using System;
using Xunit;

namespace Crank.Mapper.Tests
{
    public class MapToTests
    {
        public MapToTests()
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
        public void WhenMappingToEntities_IfAMappingExists_Success()
        {
            //given
            var userModel = CreateUserModel();

            //when mapping to an entity from a model
            var userEntity = _mapper.MapTo<UserEntity>()
                .MapFrom(userModel)
                .Result;

            //then
            Assert.NotNull(userEntity);
            Assert.Equal($"{userModel.UserId:n}", userEntity.RowKey);
            Assert.Equal(userModel.Username, userEntity.Username);
            Assert.Equal((int)userModel.UserAccess, userEntity.UserAccess);
        }

        [Fact]
        public void WhenMappingToEntities_IfAMappingDoesNotExists_ThrowANotFoundException()
        {
            //given
            UserEntity userEntity = new UserEntity();

            //when the mapping does not exists
            Assert.Throws<MappingNotFoundException<UserEntity, UserModel>>(() =>
            {
                var userModel = _mapper.MapTo<UserModel>()
                    .MapFrom(userEntity)
                    .Result;
            });
        }
    }
}
