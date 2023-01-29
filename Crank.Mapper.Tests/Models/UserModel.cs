using System;

namespace Crank.Mapper.Tests.Models
{

    public enum UserAccessModel
    {
        Disabled = 0,
        Basic = 1,
        SuperUser = 2,
        Administrator = 3
    }

    public class UserModel
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public UserAccessModel UserAccess { get; set; }
    }
}
