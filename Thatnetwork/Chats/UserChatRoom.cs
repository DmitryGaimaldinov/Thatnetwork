using Thatnetwork.Users;

namespace Thatnetwork.Chats
{
    public class UserChatRoom
    {
        public int Id { get; set; }
        public required DateTime LastReadDate { get; set; }
        public int ChatRoomId { get; set; }
        public required ChatRoom ChatRoom { get; set; }
        public int UserId {  get; set; }
        public required User User { get; set; }
    }
}
