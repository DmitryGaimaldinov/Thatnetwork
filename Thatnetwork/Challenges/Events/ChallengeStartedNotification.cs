using MediatR;
using Microsoft.AspNetCore.SignalR;
using Thatnetwork.Chats.Dtos;
using Thatnetwork.Chats;
using Thatnetwork.Challenges.Dtos;

namespace Thatnetwork.Challenges.Events
{
    public class ChallengeStartedNotification : INotification
    {
        public required MarathonDto MarathonDto { get; set; }
    }

    public class ChallengeStartedNotificationHandler : INotificationHandler<ChallengeStartedNotification>
    {
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly ILogger<ChallengeStartedNotificationHandler> _logger;
        private readonly ChatService _chatService;

        public ChallengeStartedNotificationHandler(
            IHubContext<ChatHub> chatHub,
            ILogger<ChallengeStartedNotificationHandler> logger,
            ChatService chatService)
        {
            _chatHub = chatHub;
            _logger = logger;
            _chatService = chatService;
        }

        public async Task Handle(ChallengeStartedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ChallengeStartedNotification");
            int chatRoomId = notification.MarathonDto.ChatRoomId;
            await _chatService.AddMessageAsync(
                new AddMessageDto() { ChatRoomId = chatRoomId, Text = "Марафон начался. Желаем удачи!" },
                null
            );
        }
    }

}
