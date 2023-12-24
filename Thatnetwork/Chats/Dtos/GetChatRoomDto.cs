using Thatnetwork.Chats.Validators;

namespace Thatnetwork.Chats.Dtos
{
    public class GetChatRoomDto
    {
        [AllowedChatRoom]
        public int ChatRoomId { get; set; }
    }
}
