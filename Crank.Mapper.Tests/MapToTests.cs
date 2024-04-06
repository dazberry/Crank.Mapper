using Crank.Mapper.Interfaces;
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
                new IMapping[]
                {
                    new MapUserModel_To_UserEntity(),
                    new MapUserModelAndPartitionId_To_UserEntity()
                },
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
        public void WhenMappingToEntitiesWithMapBoth_IfAMappingExists_Success()
        {
            //given
            var userModel = CreateUserModel();
            var partitionId = new PartitionId() { Value = Guid.NewGuid() };

            //when mapping to an entity from a model
            var userEntity = _mapper.MapTo<UserEntity>()
                .MapFromBoth(userModel, partitionId)
                .Result;

            //then
            Assert.NotNull(userEntity);
            Assert.Equal($"{partitionId.Value:n}", userEntity.PartitionId);
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
            Mapper mapper = CreateMapper(
                           new MapperOptions()
                           {
                               IgnoreNullResultWhenCallingDestinationMap = false
                           });

            UserModel userModel = new UserModel();

            //when/then
            var exception = Assert.Throws<MapDestinationNullResultException>(() =>
            {
                mapper.MapTo<UserModel>()
                    .Map(x =>
                    {
                        userModel = x;
                    });
            });

            Assert.Equal("The MapDestination.Result Map value is null. Invoking the MapDestination.Map delegate will pass a null value in the mapAction delegate.", exception.Message);
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

        [Fact]
        public void WhenMappingToEntities_IfCallingMapNull_ANewMapResultShouldBeCreated()
        {
            Mapper mapper = CreateMapper();
            UserModel userModel = new UserModel();

            //when
            mapper.MapNew<UserModel>()
                .Map(x =>
                {
                    userModel = x;
                });


            //then
            Assert.NotNull(userModel);
        }
    }
}
