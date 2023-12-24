using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Thatnetwork.Entities;
using Thatnetwork.Middlewares;
using Thatnetwork.Users;

namespace Thatnetwork.Chats.Validators
{
    /// <summary>
    /// Имеет ли пользователь доступ к чат-комнате
    /// </summary>

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class AllowedChatRoomAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Укажите id чат-комнаты");
            }

            int chatRoomId = (int) value;
            var httpContextAccessor = validationContext.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            var dbContext = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;

            ChatRoom? chatRoom = dbContext.ChatRooms
                .Include(cr => cr.Participants)
                .SingleOrDefault(cr => cr.Id == chatRoomId);
            if (chatRoom == null)
            {
                return new ValidationResult($"Не найден чат с id {chatRoomId}");
            }

            User currUser = httpContextAccessor.HttpContext.GetCurrentUser();
            if (!chatRoom.Participants.Any(p => p.Id == currUser.Id))
            {
                return new ValidationResult($"Вы не состоите в чате");
            }

            return ValidationResult.Success;
        }
    }
}
