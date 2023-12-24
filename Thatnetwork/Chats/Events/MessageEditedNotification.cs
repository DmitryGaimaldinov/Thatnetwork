using MediatR;
using Microsoft.AspNetCore.SignalR;
using Thatnetwork.Chats.Dtos;

namespace Thatnetwork.Chats.Events
{

    public class MessageEditedNotification : INotification
    {
        public required MessageDto MessageDto { get; set; }
        public required int ChatRoomId {  get; set; }
    }

    public class MessageEditedNotificationHandler : INotificationHandler<MessageEditedNotification>
    {
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly ILogger<MessageEditedNotification> _logger;

        public MessageEditedNotificationHandler(IHubContext<ChatHub> chatHub, ILogger<MessageEditedNotification> logger)
        {
            _chatHub = chatHub;
            _logger = logger;
        }

        public async Task Handle(MessageEditedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"MessageEditedNotificationHandler handle(). " +
                $"Send MessageEdited event to message with id {notification.MessageDto.Id}");
            await _chatHub.Clients
                .Group($"chat-room/{notification.ChatRoomId}")
                .SendAsync("MessageEdited", notification.MessageDto);
        }
    }
}
