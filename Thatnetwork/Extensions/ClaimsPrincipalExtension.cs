using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Web.Http;

namespace Thatnetwork.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static int UserId(this ClaimsPrincipal user)
        {
            string? userIdStr = user
                .Claims
                .FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase))?.Value;
            if (userIdStr == null)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            return int.Parse(userIdStr);
        }
    }
}
