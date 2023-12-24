using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using Thatnetwork.Users.Notifications;

namespace Thatnetwork.Ws
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebSocketController : ControllerBase, INotificationHandler<UserChangedNotification>
    {
        private readonly ILogger<WebSocketController> _logger;

        public WebSocketController(ILogger<WebSocketController> logger)
        {
            _logger = logger;
        }


        //[Route("/ws")]
        [HttpGet("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await Echo(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private static async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(
                    new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None);

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task Handle(UserChangedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handled UserChangedNotification with user dto name: {notification.UserDto.Name}");
        }
    }
}