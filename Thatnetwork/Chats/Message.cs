using Thatnetwork.Photos;
using Thatnetwork.Users;

namespace Thatnetwork.Chats
{
    public class Message
    {
        public int Id {  get; set; }
        public required string Text { get; set; }
        public required DateTime CreationDate { get; set; }
        public bool IsEdited { get; set; } = false;

        public int ChatRoomId { get; set; }
        public required ChatRoom ChatRoom { get; set; }
        public List<Photo> Photos { get; set; } = new();
        public int? SenderId {  get; set; }
        public required User? Sender {  get; set; }
    }
}
