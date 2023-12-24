using System.ComponentModel.DataAnnotations;
using Thatnetwork.Utils.Dtos;

namespace Thatnetwork.Chats.Dtos
{
    public class GetChatRoomsDto : PageRequestDto<DateTime?>
    {
        [Range(0, 20)]
        public override required int Count { get; set; } = 20;
    }
}
