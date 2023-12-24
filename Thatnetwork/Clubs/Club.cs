using Thatnetwork.Photos;
using Thatnetwork.Users;

namespace Thatnetwork.Clubs
{
    public class Club
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Tag { get; set; }
        public string Description { get; set; } = "";
        public int CreatorId { get; set; }
        public required User Creator { get; set; }
        public int? AvatarId { get; set; }
        public Photo? Avatar { get; set; }
    }
}
