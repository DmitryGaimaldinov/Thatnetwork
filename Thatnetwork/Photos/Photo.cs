using Thatnetwork.Users;

namespace Thatnetwork.Photos
{
    public class Photo
    {
        public int Id { get; set; }
        public required User Owner { get; set; }
        public required string Path { get; set; }
    }
}
