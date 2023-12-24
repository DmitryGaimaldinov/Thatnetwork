using System.ComponentModel.DataAnnotations;
using Thatnetwork.Entities;
using Thatnetwork.Middlewares;
using Thatnetwork.Users;

namespace Thatnetwork.Chats.Validators
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class AllowedToEditMessage : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Укажите id сообщения");
            }

            int messageId = (int)value;
            var httpContextAccessor = validationContext.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            var dbContext = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;

            Message? message = dbContext.Messages.SingleOrDefault(m => m.Id == messageId);
            if (message == null)
            {
                return new ValidationResult($"Не найдено сообщение с id {messageId}");
            }

            User currUser = httpContextAccessor.HttpContext.GetCurrentUser();
            if (message.Sender.Id != currUser.Id)
            {
                return new ValidationResult($"У вас нет доступа к изменению этого сообщения");
            }

            return ValidationResult.Success;
        }
    }
}
