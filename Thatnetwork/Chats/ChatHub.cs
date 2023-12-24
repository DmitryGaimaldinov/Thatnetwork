using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using Thatnetwork.Chats.Dtos;
using Thatnetwork.Chats.Validators;
using Thatnetwork.Users;

namespace Thatnetwork.Chats
{
    public class ChatHub : Hub
    {
        private readonly ILogger<UserHub> _logger;
        private readonly IMediator _mediator;
        private readonly ChatService _chatService;

        public ChatHub(ILogger<UserHub> logger, IMediator mediator, ChatService chatService)
        {
            _logger = logger;
            _mediator = mediator;
            _chatService = chatService;
        }

        //// Пользователь подписывается и получает ChatRoomDto с последним сообщением
        //public async Task ListenChatRooms()
        //{
        //    // Получаем UserId
        //    int userId;
        //    // Добавляем в группу прослушивальщиков сообщений
        //    await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-rooms-listeners/{userId}");
        //    // Прослушиваем сообщение
        //    // MessageSentEvent.data = { MessageDto messageDto, ChatRoomDto chatRoomDto }
        //    // _chatService.GetMembers()
        //    // Отправляем сообщение
        //    await this.Clients.Group($"chat-rooms-listeners/{userId}").SendAsync("message-received", chatRoomDto);

        //    await this.Clients.All.SendAsync("jopa", $"Лови сообщение {message}");
        //}

        //public async Task ListenUnreadMessagesCount()
        //{

        //}



        // [AllowedChatRoom]
        public async Task ListenRoom(int chatRoomId)
        {
            _logger.LogWarning($"User is listening room with id {chatRoomId}");
            // TODO: Валидировать, имеет ли пользователь право на прослушку группы
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-room/{chatRoomId}");
        }

        public async Task UnlistenRoom(int chatRoomId)
        {

        }
    }
}
