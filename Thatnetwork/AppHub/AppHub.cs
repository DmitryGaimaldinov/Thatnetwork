using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using Thatnetwork.Chats;
using Thatnetwork.Users;

namespace Thatnetwork.AppHub
{
    public class AppHub : Hub
    {
        private readonly ILogger<AppHub> _logger;
        private readonly IMediator _mediator;
        private readonly ChatService _chatService;
        private readonly UserService _userService;

        public AppHub(
            ILogger<AppHub> logger, 
            IMediator mediator, 
            ChatService chatService,
            UserService userService
        ) {
            _logger = logger;
            _mediator = mediator;
            _chatService = chatService;
            _userService = userService;
        }

        public void Login(string accessToken)
        {
            JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            int userId = int.Parse(token.Subject);
            _logger.LogCritical($"AppHub login. userId: {userId}");
            Context.Items["userId"] = userId;
        }

        public void Logout(string accessToken)
        {
            Context.Items["userId"] = null;
        }

        public async Task ListenRoom(int chatRoomId)
        {
            _logger.LogWarning($"User is listening room with id {chatRoomId}");
            // TODO: Валидировать, имеет ли пользователь право на прослушку группы
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-room/{chatRoomId}");
        }
    }
}
