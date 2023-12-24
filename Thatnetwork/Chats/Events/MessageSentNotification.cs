using MediatR;
using Microsoft.AspNetCore.SignalR;
using Thatnetwork.Chats.Dtos;
using Thatnetwork.Users;

namespace Thatnetwork.Chats.Events
{
    public class MessageSentNotification : INotification
    {
        public required MessageDto MessageDto { get; set; }
        public required int ChatRoomId {  get; set; }
    }

    public class MessageSentNotificationHandler : INotificationHandler<MessageSentNotification>
    {
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly ILogger<MessageSentNotificationHandler> _logger;
        private readonly ChatService _chatService;

        public MessageSentNotificationHandler(
            IHubContext<ChatHub> chatHub, 
            ILogger<MessageSentNotificationHandler> logger,
            ChatService chatService
        ) { 
            _chatHub = chatHub;
            _logger = logger;
            _chatService = chatService;
        }

        public async Task Handle(MessageSentNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"MessageSentNotificationHandler handle(). " +
                $"Send NewMessage event to chat room with id {notification.ChatRoomId}. " +
                $"new message id: ${notification.MessageDto.Id}");
            await _chatHub.Clients
                .Group($"chat-room/{notification.ChatRoomId}")
                .SendAsync("NewMessage", notification.MessageDto);
        }
    }
}
