namespace Crank.Mapper.Tests.Models
{
    public class UserEntity
    {
        public string PartitionId { get; set; }
        public string RowKey { get; set; }
        public string Username { get; set; }
        public int UserAccess { get; set; }
    }
}
