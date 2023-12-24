using Thatnetwork.Photos.Dtos;

namespace Thatnetwork.Users.Dtos
{
    public class UserDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Tag { get; set; }
        public required string Description {  get; set; }
        public required string? AvatarPath {  get; set; }
        public required string? BackgroundPath {  get; set; }
    }
}
