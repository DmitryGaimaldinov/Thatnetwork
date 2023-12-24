using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thatnetwork.Chats.Dtos;
using Thatnetwork.Middlewares;
using Thatnetwork.Users;

namespace Thatnetwork.Chats
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("send-message"), Authorize]
        public async Task<ActionResult> SendMessage(AddMessageDto dto)
        {
            User currUser = HttpContext.GetCurrentUser();

            await _chatService.AddMessageAsync(dto, currUser);

            return Ok();
        }

        [HttpPut("edit-message"), Authorize]
        public async Task<ActionResult> EditMessage(EditMessageDto dto)
        {
            User currUser = HttpContext.GetCurrentUser();
            await _chatService.EditMessageAsync(dto);
            return Ok();
        }


        [HttpGet("chat-rooms"), Authorize]
        public async Task<ActionResult<List<ChatRoomDto>>> GetChatRooms()
        {
            User currUser = HttpContext.GetCurrentUser();

            return Ok(await _chatService.GetChatRoomsAsync(currUser.Id));
        }

        [HttpPost("get-chat-room"), Authorize]
        public async Task<ActionResult<List<ChatRoomDto>>> GetChatRoom(GetChatRoomDto dto)
        {
            User currUser = HttpContext.GetCurrentUser();

            return Ok(await _chatService.GetChatRoomByIdAsync(dto.ChatRoomId, currUser.Id));
        }

        [HttpPost("get-messages"), Authorize]
        public async Task<ActionResult<MessagePageDto>> GetMessages(GetMessagesDto dto)
        {
            //User currUser = HttpContext.GetCurrentUser();

            return await _chatService.GetMessagePageAsync(dto);
        }
    }
}
