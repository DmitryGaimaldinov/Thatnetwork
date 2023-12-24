using Thatnetwork.Clubs;
using Thatnetwork.Photos;
using Thatnetwork.Users;

namespace Thatnetwork.Notes
{
    public class Note
    {
        public int Id { get; set; }
        public int CreatorId { get; set; }
        public required User Creator { get; set; }
        public required string Text { get; set; }
        public List<Photo> Photos { get; set; } = new();
        public Note? Parent { get; set; }
        public List<Note> Children { get; set; } = new();
        public int? ClubId {  get; set; }
        public Club? Club { get; set; }
    }
}
