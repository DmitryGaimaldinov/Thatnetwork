using MediatR;
using Microsoft.AspNetCore.SignalR;
using Thatnetwork.Users.Notifications;

namespace Thatnetwork.Users.NotificationHandlers
{
    public class UserChangedNotificationHandler : INotificationHandler<UserChangedNotification>
    {
        private readonly IHubContext<UserHub> _userHub;
        private readonly ILogger<UserChangedNotificationHandler> _logger;

        public UserChangedNotificationHandler(IHubContext<UserHub> userHub, ILogger<UserChangedNotificationHandler> logger)
        {
            _userHub = userHub;
            _logger = logger;
        }

        public async Task Handle(UserChangedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"UserChangedNotification handled. UserId: {notification.UserDto.Id}");
            await _userHub.Clients
                .Group($"user/{notification.UserDto.Id}")
                .SendAsync("UserChanged", notification.UserDto);
        }
    }
}
