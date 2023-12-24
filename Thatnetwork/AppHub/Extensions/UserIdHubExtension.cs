using Microsoft.AspNetCore.SignalR;
using Thatnetwork.Users;

namespace Thatnetwork.AppHub.Extensions
{
    public static class UserIdHubExtension
    {
        public static int? GetCurrentUser(this HubCallerContext context)
        {
            return (int?) context.Items["userId"];
        }
    }
}
