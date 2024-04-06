using Crank.Mapper.Interfaces;
using Crank.Mapper.Tests.Models;

namespace Crank.Mapper.Tests.Mappings
{
    public class MapUserModelAndPartitionId_To_UserEntity : IMapping<UserModel, PartitionId, UserEntity>
    {
        public UserEntity Map(UserModel source, PartitionId source2, UserEntity destination)
        {
            destination ??= new UserEntity();

            destination.PartitionId = $"{source2.Value:n}";
            destination.RowKey = $"{source.UserId:n}";
            destination.Username = source.Username;
            destination.UserAccess = (int)source.UserAccess;

            return destination;
        }
    }
}
