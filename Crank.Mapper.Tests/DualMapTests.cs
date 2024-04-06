using Crank.Mapper.Tests.Mappings;
using Crank.Mapper.Tests.Models;
using System;
using Xunit;

namespace Crank.Mapper.Tests
{
    public class DualMapTests
    {
        public DualMapTests()
        {
            _mapper = new Mapper(
                new[] { new MapUserModelAndPartitionId_To_UserEntity() });
        }

        private readonly Mapper _mapper;

        private static UserModel CreateUserModel() =>
            new UserModel()
            {
                UserId = Guid.NewGuid(),
                Username = "username123",
                UserAccess = UserAccessModel.Basic,
            };

        private static PartitionId CreatePartitionId() =>
            new PartitionId()
            {
                Value = Guid.NewGuid()
            };

        [Fact]
        public void WhenMappingEntities_IfAMappingExists_ShouldReturnDestinationEntity()
        {
            //given
            var userModel = CreateUserModel();
            var partitionId = CreatePartitionId();

            //when
            var userEntity = _mapper.Map<UserModel, PartitionId, UserEntity>(userModel, partitionId);

            //then
            Assert.NotNull(userEntity);
            Assert.Equal($"{partitionId.Value:n}", userEntity.PartitionId);
            Assert.Equal($"{userModel.UserId:n}", userEntity.RowKey);
            Assert.Equal(userModel.Username, userEntity.Username);
            Assert.Equal((int)userModel.UserAccess, userEntity.UserAccess);
        }

        [Fact]
        public void WhenMappingEntities_IfAMappingDoesNotExist_ReturnANullEntity()
        {
            //given
            var userEntity = new UserEntity();
            var partitionId = CreatePartitionId();

            //when the mapping does not exist
            var userModel = _mapper.Map<UserEntity, PartitionId, UserModel>(userEntity, partitionId);

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
            var partitionId = CreatePartitionId();

            //when the mapping does not exist
            Assert.Throws<MappingNotFoundException<UserEntity, UserModel>>(() =>
            {
                var userModel = mapper.Map<UserEntity, PartitionId, UserModel>(userEntity, partitionId);
            });
        }
    }
}
