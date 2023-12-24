using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Thatnetwork.Users
{
    public class UserHub : Hub
    {
        private readonly ILogger<UserHub> _logger;
        private readonly IMediator _mediator;
        private readonly UserService _userService;

        public UserHub(ILogger<UserHub> logger, IMediator mediator, UserService userService)
        {       
            _logger = logger;
            _mediator = mediator;
            _userService = userService;
        }

        public async Task ListenUser(int userId)
        {
            _logger.LogInformation($"user with id {userId} is now being listened");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user/{userId}");
        }

        public async Task StopListeningUser(int userId)
        {
            _logger.LogInformation($"user with id {userId} is stopped being listened");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user/{userId}");
        }
    }
}
