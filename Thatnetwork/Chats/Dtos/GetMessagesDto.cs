using System.ComponentModel.DataAnnotations;
using Thatnetwork.Chats.Validators;
using Thatnetwork.Utils.Dtos;

namespace Thatnetwork.Chats.Dtos
{
    public class GetMessagesDto : PageRequestDto<DateTime?>
    {
        [AllowedChatRoom]
        public required int ChatRoomId {  get; set; }

        [Range(0, 30)]
        public override int Count { get; set; } = 30;
    }
}
