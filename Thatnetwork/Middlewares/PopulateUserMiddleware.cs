using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Thatnetwork.Entities;
using Thatnetwork.Users;

namespace Thatnetwork.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class PopulateUserMiddleware
    {
        private readonly RequestDelegate _next;

        public PopulateUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, AppDbContext dbContext, ILogger<PopulateUserMiddleware> logger)
        {
            string? userIdStr = httpContext.User
                .Claims
                .FirstOrDefault(c => c.Type.Equals(JwtRegisteredClaimNames.Sub, StringComparison.OrdinalIgnoreCase))?.Value;
            if (userIdStr != null)
            {
                int userId = int.Parse(userIdStr);
                var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
                httpContext.Items["CurrentUser"] = user;
                logger.LogInformation($"user set to httpContext.Items. User: {user}");
            }
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CurrentUserExtensions
    {
        public static User GetCurrentUser(this HttpContext context)
        {
            return (User) context.Items["CurrentUser"]!;
        }

        public static User? GetCurrentUserOrNull(this HttpContext context)
        {
            return (User?) context.Items["CurrentUser"];
        }

        //public static IApplicationBuilder UseCurrentUser(this IApplicationBuilder builder)
        //{
        //    return builder.UseMiddleware<PopulateUserMiddleware>();
        //}
    }
}
