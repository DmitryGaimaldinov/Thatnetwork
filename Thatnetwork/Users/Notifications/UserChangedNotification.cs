using MediatR;
using Thatnetwork.Users.Dtos;

namespace Thatnetwork.Users.Notifications
{
    public class UserChangedNotification : INotification
    {
        public required UserDto UserDto { get; set; }
    }
}
