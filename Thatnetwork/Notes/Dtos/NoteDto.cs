using Thatnetwork.Entities;
using Thatnetwork.Photos.Dtos;
using Thatnetwork.Users.Dtos;

namespace Thatnetwork.Notes.Dtos
{
    public class NoteDto
    {
        public int Id { get; set; }
        public required UserDto Creator { get; set; }
        public required string Text { get; set; }
        public List<PhotoDto> Photos { get; set; } = new();
        public required int CommentCount {  get; set; }
    }
}
