using Thatnetwork.Photos.Dtos;

namespace Thatnetwork.Clubs.Dtos
{
    public class ClubDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Tag { get; set; }
        public required string Description { get; set; }
        public PhotoDto? Avatar { get; set; }
    }
}
