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
            _mapper = CreateMapper();
        }

        private Mapper CreateMapper(MapperOptions mapperOptions = default) =>
            new Mapper(
                new[] { new MapUserModel_To_UserEntity() },
                mapperOptions);


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

        [Fact]
        public void WhenMappingToEntities_ANullResultWhenInvokingMap_ThrowANullArgumentException()
        {
            //given
            UserModel userModel = new UserModel();

            //when
            Assert.Throws<ArgumentNullException>(() =>
            {
                _mapper.MapTo<UserModel>()
                    .Map(x =>
                    {
                        userModel = x;
                    });
            });
            Assert.NotNull(userModel);
        }

        [Fact]
        public void WhenMappingToEntities_ANullResultWhenInvokingMapWithTheIgnoreNullFlagSet_DoesNotThrowAnException()
        {            //given

            Mapper mapper = CreateMapper(
                new MapperOptions()
                {
                    IgnoreNullResultWhenCallingDestinationMap = true
                });

            UserModel userModel = new UserModel();

            //when
            mapper.MapTo<UserModel>()
                .Map(x =>
                {
                    userModel = x;
                });


            //then
            Assert.Null(userModel);
        }
    }
}
