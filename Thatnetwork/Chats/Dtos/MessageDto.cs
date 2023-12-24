using Thatnetwork.Photos;
using Thatnetwork.Photos.Dtos;
using Thatnetwork.Users.Dtos;

namespace Thatnetwork.Chats.Dtos
{
    public class MessageDto
    {
        public required int Id { get; set; }
        public required string Text { get; set; }
        public required DateTime CreationDate { get; set; }
        public List<PhotoDto> Photos { get; set; } = new();
        public required UserDto? Sender { get; set; }
        public required bool IsEdited {  get; set; }
    }
}
