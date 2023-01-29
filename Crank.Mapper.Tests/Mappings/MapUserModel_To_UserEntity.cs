using Crank.Mapper.Interfaces;
using Crank.Mapper.Tests.Models;

namespace Crank.Mapper.Tests.Mappings
{
    public class MapUserModel_To_UserEntity : IMapping<UserModel, UserEntity>
    {
        public UserEntity Map(UserModel source, UserEntity destination = null)
        {
            destination ??= new UserEntity();

            destination.PartitionId = "user";
            destination.RowKey = $"{source.UserId:n}";
            destination.Username = source.Username;
            destination.UserAccess = (int)source.UserAccess;

            return destination;
        }
    }
}
